using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace Osmo.ConneX.Consumers;

using Common.Messages;
using Models;
using Providers;
using Services;

/// <summary>
/// Consumer of MassTransit messages of type <see cref="MqttMessage"/> and <see cref="BulkMqttMessage"/>.
/// </summary>
internal class MqttMessageConsumer : IConsumer<MqttMessage>, IConsumer<BulkMqttMessage>
{
    private readonly MqttRouter _router = new();
    private readonly ConneXRecordInjesterService _recordInjesterService;
    private readonly IDbContextFactory<ConneXMetricsProviderContext> _contextFactory;
    private readonly ILogger<MqttMessageConsumer> _logger;

    /// <summary>
    /// Creates a new instance of <see cref="MqttMessageConsumer"/>.
    /// </summary>
    /// <param name="recordInjesterService">An instance of <see cref="ConneXRecordInjesterService"/>.</param>
    /// <param name="contextFactory">An instance of <see cref="IDbContextFactory{TContext}"/> for <see cref="ConneXMetricsProviderContext"/>.</param>
    /// <param name="logger">An instance of <see cref="ILogger{TCategoryName}"/>.</param>
    public MqttMessageConsumer(ConneXRecordInjesterService recordInjesterService, 
        IDbContextFactory<ConneXMetricsProviderContext> contextFactory,
        ILogger<MqttMessageConsumer> logger)
    {
        _recordInjesterService = recordInjesterService;
        _contextFactory = contextFactory;
        _logger = logger;

        _router.RegisterRoute("connex/programmer/#", ProcessAuditRecord);
    }
    
    /// <summary>
    /// Consumes a <see cref="MqttMessage"/> message.
    /// </summary>
    /// <param name="context">The <see cref="ConsumeContext{T}"/> of type <see cref="MqttMessage"/>.</param>
    public async Task Consume(ConsumeContext<MqttMessage> context)
    {
        await using var dbContext = await _contextFactory.CreateDbContextAsync(context.CancellationToken);

        var message = await dbContext.MqttMessages.FirstOrDefaultAsync(x => x.Id == context.MessageId, context.CancellationToken);
        if (message is not null)
        {
            return;
        }
        
        await dbContext.MqttMessages.AddAsync(context.Message, context.CancellationToken);
        await dbContext.SaveChangesAsync(context.CancellationToken);
        
        await _router.RouteMessage(context.Message.Topic, context.Message);
    }

    /// <summary>
    /// Consumes a <see cref="BulkMqttMessage"/> message.
    /// </summary>
    /// <param name="context">The <see cref="ConsumeContext{T}"/> of type <see cref="BulkMqttMessage"/>.</param>
    /// <remarks>Messages are not saved to the database if they already exist.</remarks>
    public async Task Consume(ConsumeContext<BulkMqttMessage> context)
    {
        await using var dbContext = await _contextFactory.CreateDbContextAsync(context.CancellationToken);
        
        await dbContext.MqttMessages.AddRangeAsync(context.Message.Messages, context.CancellationToken);
        await dbContext.SaveChangesAsync(context.CancellationToken);
        
        foreach (var message in context.Message.Messages)
        {
            await _router.RouteMessage(message.Topic, message);
        }
    }

    /// <summary>
    /// Processes an audit record.
    /// </summary>
    /// <param name="message">A <see cref="MqttMessage"/> message that contains an audit record.</param>
    private async Task ProcessAuditRecord(MqttMessage message)
    {
        List<ConneXAuditEntry> auditEntries;
        try
        {
            auditEntries = DeserializePayload(message.PayloadAsString);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Failed to deserialize payload {Payload}", message.PayloadAsString);
            return;
        }
        
        foreach (var entry in auditEntries)
        {
            await _recordInjesterService.AddRecord(entry);
        }
    }
    
    /// <summary>
    /// Deserializes the payload of a message into a list of <see cref="ConneXAuditEntry"/> objects.
    /// </summary>
    /// <param name="payload">The payload to deserialize.</param>
    /// <returns>A collection of <see cref="ConneXAuditEntry"/>.</returns>
    private List<ConneXAuditEntry> DeserializePayload(string payload)
    {
        JsonSerializerOptions jsonSerializerOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };
        
        try
        {
            return JsonSerializer.Deserialize<List<ConneXAuditEntry>>(payload, jsonSerializerOptions);
        }
        catch (Exception)
        {
            // If the payload is not an array, try to deserialize it as a single object.
        }

        return [JsonSerializer.Deserialize<ConneXAuditEntry>(payload, jsonSerializerOptions)];
    }
}