using MassTransit;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Components.Server;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Osmo.ConneX.Extensions;

using Common.Plugins;
using Consumers;
using Osmo.Common.Database.Extensions;
using Providers;
using Services;
using Ui;
using Ui.MessageViewer;

/// <summary>
/// Extension methods for the ConneX plugin.
/// </summary>
public static class ConneXPluginExtensions
{
    /// <summary>
    /// Adds the ConneX plugin to the service collection.
    /// </summary>
    /// <param name="serviceCollection">The service collection to add the plugin to.</param>
    /// <param name="configuration">The configuration to use.</param>
    public static void AddConneXPlugin(this IServiceCollection serviceCollection, IConfiguration configuration)
    {
        ConneXOptions options = configuration.GetSection(nameof(ConneXOptions)).Get<ConneXOptions>();
        serviceCollection.Configure<ConneXOptions>(configuration.GetSection(nameof(ConneXOptions)).Bind);
        
        serviceCollection.AddTransient<IOsmoPlugin, ConneXPlugin>();

        serviceCollection.AddSingleton<ConneXRecordInjesterService>();
        serviceCollection.AddHostedService(s => s.GetRequiredService<ConneXRecordInjesterService>());
        serviceCollection.AddHostedService<ConneXMqttService>();
        serviceCollection.AddHostedService<ConneXSynchronization>();

        serviceCollection.AddScoped<TimeSeriesJavascriptInterop>();

        serviceCollection.AddConneXClient()
            .ConfigureHttpClient(
                client => client.BaseAddress = new Uri($"http://{options.ConneXHost.HostName}:5001/graphql"));
        
        serviceCollection.AddDbContextFactory<ConneXMetricsProviderContext, OsmoConnexMetricsProviderContextFactory>();
    }
    
    /// <summary>
    /// Add ConneX root components to the circuit options.
    /// </summary>
    /// <param name="options">The circuit options to add the components to.</param>
    public static void AddConneXRootComponents(this CircuitOptions options)
    {
        options.RootComponents.RegisterForJavaScript<ConneXAnalyzer>("connex-analyzer");
        options.RootComponents.RegisterForJavaScript<ConneXDashboard>("connex-dashboard");
        options.RootComponents.RegisterForJavaScript<ConneXHandler>("connex-handler");
        options.RootComponents.RegisterForJavaScript<ConneXMetrics>("connex-metrics");
        options.RootComponents.RegisterForJavaScript<ConneXMessageList>("connex-message-list");
        options.RootComponents.RegisterForJavaScript<RecordIngester>("connex-record-ingester");
        options.RootComponents.RegisterForJavaScript<DeviceList>("connex-device-list");
        options.RootComponents.RegisterForJavaScript<JobList>("connex-job-list");
        options.RootComponents.RegisterForJavaScript<DeviceDetails>("connex-device-details");
        options.RootComponents.RegisterForJavaScript<JobDetails>("connex-job-details");
    }
    
    /// <summary>
    /// Configures the ConneX metrics database.
    /// </summary>
    /// <param name="applicationBuilder"></param>
    public static async Task ConfigureConneXMetricsDatabase(this IApplicationBuilder applicationBuilder)
    {
        var dbFactory = applicationBuilder.ApplicationServices.GetRequiredService<IDbContextFactory<ConneXMetricsProviderContext>>();

        await using var connexMetricsDb = await dbFactory.CreateDbContextAsync();
     
        await connexMetricsDb.Database.MigrateAsync();
        connexMetricsDb.ApplyHypertables();
    }

    /// <summary>
    /// Add MassTransit consumers for the ConneX plugin.
    /// </summary>
    /// <param name="configurator"></param>
    public static void AddConneXBusConsumers(this IBusRegistrationConfigurator configurator)
    {
        configurator.AddConsumer<MqttMessageConsumer>();
        configurator.AddConsumers(typeof(ConneXPluginExtensions).Assembly);
    }
}