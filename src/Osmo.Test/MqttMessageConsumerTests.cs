using MassTransit.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Osmo.Common.Database.Options;
using Osmo.ConneX;
using Osmo.ConneX.Consumers;
using Osmo.ConneX.Consumers.Processors;
using Osmo.ConneX.Providers;

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
        
        // var query = dbContext.MqttMessages
        //     .Where(m => m.Topic.Contains("h700"))
        //     .OrderBy(m => m.Timestamp)
        //     .AsQueryable();
        
        var lastEntry = await dbContext.HandlerEvents
            .OrderByDescending(m => m.Timestamp)
            .FirstOrDefaultAsync();
        
        var query = dbContext.MqttMessages
            .AsNoTracking()
            .Where(m => EF.Functions.ILike(m.Topic, "%h700%"))
            .Where(m => m.Timestamp > lastEntry.Timestamp)
            .OrderBy(m => m.Timestamp)
            .AsQueryable();

        int count = await query.CountAsync();
        int progress = 0;

        await foreach (var message in query.AsAsyncEnumerable())
        {
            try
            {
                await handlerEventProcessor.ProcessEvent(message);
                progress++;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                Console.WriteLine(message.PayloadAsString);
            }

        }
    }
}