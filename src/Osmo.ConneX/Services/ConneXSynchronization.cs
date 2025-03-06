using MassTransit;
using MassTransit.Initializers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace Osmo.ConneX.Services;

using Common.Messages;
using GraphQl;
using Models;
using Providers;

/// <summary>
/// Hosted service for synchronizing with ConneX.
/// </summary>
internal class ConneXSynchronization : IHostedService
{
    private const string _lastMessageTimestamp = "last_message_timestamp";
    
    private readonly IDbContextFactory<ConneXMetricsProviderContext> _connexMetricsProviderContextFactory;
    private readonly IConneXClient _connexGraphql;
    private readonly IBus _massTransitBus;
    private readonly ILogger<ConneXSynchronization> _logger;
    
    private Task _connexSyncWorker;
    private CancellationTokenSource _cancellationTokenSource;

    /// <summary>
    /// Creates a new instance of the <see cref="ConneXSynchronization"/> class.
    /// </summary>
    /// <param name="connexMetricsProviderContextFactory">A factory for creating instances of the <see cref="ConneXMetricsProviderContext"/> class.</param>
    /// <param name="connexGraphql">A client for interacting with the ConneX GraphQL API.</param>
    /// <param name="massTransitBus">A MassTransit bus for publishing messages.</param>
    /// <param name="logger">An instance of the <see cref="ILogger{TCategoryName}"/> class.</param>
    public ConneXSynchronization(IDbContextFactory<ConneXMetricsProviderContext> connexMetricsProviderContextFactory,
        IConneXClient connexGraphql,
        IBus massTransitBus,
        ILogger<ConneXSynchronization> logger)
    {
        _connexMetricsProviderContextFactory = connexMetricsProviderContextFactory;
        _connexGraphql = connexGraphql;
        _massTransitBus = massTransitBus;
        _logger = logger;
    }
    
    /// <summary>
    /// Triggered when the application host is ready to start the service.
    /// </summary>
    /// <param name="cancellationToken">Indicates that the start process has been aborted.</param>
    /// <returns>A <see cref="T:System.Threading.Tasks.Task" /> that represents the asynchronous Start operation.</returns>
    public Task StartAsync(CancellationToken cancellationToken)
    {
        _cancellationTokenSource = new CancellationTokenSource();
        
        _connexSyncWorker = Task.Run(SyncWithConneX, _cancellationTokenSource.Token);
        
        return Task.CompletedTask;
    }

    /// <summary>
    /// Triggered when the application host is performing a graceful shutdown.
    /// </summary>
    /// <param name="cancellationToken">Indicates that the shutdown process should no longer be graceful.</param>
    /// <returns>A <see cref="T:System.Threading.Tasks.Task" /> that represents the asynchronous Stop operation.</returns>
    public async Task StopAsync(CancellationToken cancellationToken)
    {
        await _cancellationTokenSource.CancelAsync();
        await _connexSyncWorker;
        
        _cancellationTokenSource.Dispose();
    }
    
    /// <summary>
    /// Worker method for syncing with ConneX.
    /// </summary>
    private async Task SyncWithConneX()
    {
        while (!_cancellationTokenSource.Token.IsCancellationRequested)
        {
            try
            {
                await Task.Delay(TimeSpan.FromMinutes(5));
                await IngestAllRecords();
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error syncing with ConneX");
            }
        }
    }

    /// <summary>
    /// Ingests all records from ConneX.
    /// </summary>
    private async Task IngestAllRecords()
    {
        await using var dbContext = await _connexMetricsProviderContextFactory.CreateDbContextAsync(_cancellationTokenSource.Token);
        string lastMessageTimestamp = await dbContext.ConneXMetaData.FirstOrDefaultAsync(m => m.Key == _lastMessageTimestamp).Select(m => m.Value);

        DateTimeOffset lastMessageTimestampParsed;
        if (string.IsNullOrWhiteSpace(lastMessageTimestamp))
        {
            lastMessageTimestampParsed = DateTimeOffset.UnixEpoch;
        }
        else
        {
            lastMessageTimestampParsed = DateTimeOffset.Parse(lastMessageTimestamp);
        }
        
        int skip = 0;
        int take = 50;
        bool hasNextPage = true;

        while (hasNextPage)
        {
            var pageResult = await _connexGraphql.GetAllMessages.ExecuteAsync(skip, take, lastMessageTimestampParsed);
            
            if (pageResult.Data?.Messages is null)
            {
                await Task.Delay(50);
                continue;
            }
            
            await ProcessItems(pageResult.Data.Messages.Items, _cancellationTokenSource.Token);
            
            hasNextPage = pageResult.Data?.Messages?.PageInfo?.HasNextPage ?? false;
            skip += take;
        }
    }

    /// <summary>
    /// Processes a page worth of messages.
    /// </summary>
    /// <param name="items">A collection of messages.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    private async Task ProcessItems(IReadOnlyList<IGetAllMessages_Messages_Items> items, CancellationToken cancellationToken)
    {
        if (items is null)
        {
            return;
        }
        
        foreach (var message in items)
        {
            await _massTransitBus.Publish(new MqttMessage()
            {
                Id = message.MessageModelId,
                Topic = message.Topic,
                PayloadAsString = message.PayloadAsString,
                ContentType = message.ContentType,
                Timestamp = message.Timestamp.UtcDateTime
            }, cancellationToken);
        }

        await UpdateLastMessageTimestamp(items[^1].Timestamp);
    }
    
    /// <summary>
    /// Updates the last message timestamp.
    /// </summary>
    /// <param name="timestamp">The timestamp of the last message.</param>
    private async Task UpdateLastMessageTimestamp(DateTimeOffset timestamp)
    {
        await using var dbContext = await _connexMetricsProviderContextFactory.CreateDbContextAsync(_cancellationTokenSource.Token);
        
        var lastMessageTimestamp = await dbContext.ConneXMetaData.FirstOrDefaultAsync(m => m.Key == _lastMessageTimestamp);
        
        if (lastMessageTimestamp is null)
        {
            dbContext.ConneXMetaData.Add(new ConneXMetaData
            {
                Key = _lastMessageTimestamp,
                Value = timestamp.ToUniversalTime().ToString()
            });
        }
        else
        {
            lastMessageTimestamp.Value = timestamp.ToUniversalTime().ToString();
        }
        
        await dbContext.SaveChangesAsync(_cancellationTokenSource.Token);
    }
}