using MassTransit.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Osmo.Common.Database.Options;
using Osmo.Common.Messages;
using Osmo.ConneX;
using Osmo.ConneX.Consumers;
using Osmo.ConneX.Consumers.Processors;
using Osmo.ConneX.Models;
using Osmo.ConneX.Providers;
using Osmo.ConneX.Services;
using Osmo.Database;
using System.Text.Json;
using System.Threading.Channels;

namespace Osmo.Test;

public class MqttMessageConsumerTests
{
    [Test]
    public async Task ImportFromConneX()
    {
        IOptions<PostgresOptions> options = new OptionsWrapper<PostgresOptions>(new PostgresOptions()
        {
            Host = "127.0.0.1",
            Port = 6432,
            Username = "admin",
            Password = "admin"
        });
        var conneXMetricsProviderContextFactory = new OsmoConnexMetricsProviderContextFactory(options);
        var handlerEventProcessor = new HandlerEventProcessor(conneXMetricsProviderContextFactory);

        await using var dbContext = conneXMetricsProviderContextFactory.CreateDbContext();

        var lastEntry = await dbContext.HandlerEvents
            .OrderByDescending(m => m.Timestamp)
            .FirstOrDefaultAsync();

        lastEntry.Timestamp = DateTimeOffset.MinValue.DateTime;

        var query = dbContext.MqttMessages
            .AsNoTracking()
            .Where(m => EF.Functions.ILike(m.Topic, "%h700%"))
            .Where(m => m.Timestamp > lastEntry.Timestamp)
            .OrderBy(m => m.Timestamp)
            .AsQueryable();

        int count = await query.CountAsync();
        int progress = 0;

        List<Task> workerTasks = new List<Task>();
        CancellationTokenSource cts = new CancellationTokenSource();
        var channel = Channel.CreateBounded<MqttMessage>(100);
        for (int i = 0; i < 8; i++)
        {
            workerTasks.Add(Task.Run(async () => await ProcessEventWorker(cts.Token, handlerEventProcessor, channel),
                cts.Token));
        }

        await foreach (var message in query.AsAsyncEnumerable())
        {
            await channel.Writer.WriteAsync(message, cts.Token);
            progress++;
        }

        while (channel.Reader.Count > 0)
        {
            await Task.Delay(100);
        }

        await cts.CancelAsync();
        await Task.WhenAll(workerTasks);
    }

    [Test]
    public async Task ImportLumenXJobEventsFromConneX()
    {
        IOptions<PostgresOptions> options = new OptionsWrapper<PostgresOptions>(new PostgresOptions()
        {
            Host = "127.0.0.1",
            Port = 6432,
            Username = "admin",
            Password = "admin"
        });
        var osmoContextFactory = new OsmoContextFactory(options);
        var conneXMetricsProviderContextFactory = new OsmoConnexMetricsProviderContextFactory(options);
        var jobEventProcessor = new JobEventProcessor(conneXMetricsProviderContextFactory, osmoContextFactory);

        await using var dbContext = conneXMetricsProviderContextFactory.CreateDbContext();
        await dbContext.Database.MigrateAsync();

        var lastEntry = await dbContext.HandlerEvents
            .OrderByDescending(m => m.Timestamp)
            .FirstOrDefaultAsync();

        lastEntry.Timestamp = DateTimeOffset.MinValue.DateTime;

        var query = dbContext.MqttMessages
            .AsNoTracking()
            .Where(m => m.Topic == "connex/programmer/lumenx/legacy/programmingcomplete")
            .Where(m => m.Timestamp > lastEntry.Timestamp)
            .OrderBy(m => m.Timestamp)
            .AsQueryable();

        int count = await query.CountAsync();
        int progress = 0;

        List<Task> workerTasks = new List<Task>();
        CancellationTokenSource cts = new CancellationTokenSource();
        var channel = Channel.CreateBounded<MqttMessage>(100);
        for (int i = 0; i < 8; i++)
        {
            workerTasks.Add(Task.Run(
                async () => await ProcessLumenXEventWorker(cts.Token, jobEventProcessor, channel),
                cts.Token));
        }

        await foreach (var message in query.AsAsyncEnumerable())
        {
            await channel.Writer.WriteAsync(message, cts.Token);
            progress++;
        }

        while (channel.Reader.Count > 0)
        {
            await Task.Delay(100);
        }

        await cts.CancelAsync();
        await Task.WhenAll(workerTasks);
    }

    private static async Task ProcessEventWorker(CancellationToken cancellationToken,
        HandlerEventProcessor handlerEventProcessor, Channel<MqttMessage> channel)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            MqttMessage? message = null;
            try
            {
                message = await channel.Reader.ReadAsync(cancellationToken);
                await handlerEventProcessor.ProcessEvent(message);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                Console.WriteLine(message?.PayloadAsString);
            }
        }
    }

    private static async Task ProcessLumenXEventWorker(CancellationToken cancellationToken,
        JobEventProcessor jobEventProcessor, Channel<MqttMessage> channel)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            MqttMessage? message = null;
            try
            {
                message = await channel.Reader.ReadAsync(cancellationToken);
                await jobEventProcessor.ProcessEvent(message);

                // foreach (var record in DeserializePayload(message.PayloadAsString))
                // {
                //     await jobEventProcessor.ProcessEvent(record);
                // }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                Console.WriteLine(message?.PayloadAsString);
            }
        }
    }
}