using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Components.Server;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Osmo.Common.Database.Extensions;
using Osmo.Common.Plugins;
using Osmo.ConneX.Providers;
using Osmo.ConneX.Services;
using Osmo.ConneX.Ui;
using Osmo.ConneX.Ui.MessageViewer;
using Osmo.Database;

namespace Osmo.ConneX.Extensions;

public static class ConneXPluginExtensions
{
    public static void AddConneXPlugin(this IServiceCollection serviceCollection, IConfiguration configuration)
    {
        ConneXOptions options = configuration.GetSection("ConneXOptions").Get<ConneXOptions>();
        
        serviceCollection.AddTransient<IOsmoPlugin, ConneXPlugin>();

        serviceCollection.AddSingleton<ConneXRecordInjesterService>();
        serviceCollection.AddHostedService(s => s.GetRequiredService<ConneXRecordInjesterService>());

        serviceCollection.AddScoped<TimeSeriesJavascriptInterop>();

        // serviceCollection.AddSingleton<ConneXMqttService>();
        // serviceCollection.AddHostedService(s => s.GetRequiredService<ConneXMqttService>());

        serviceCollection.AddConneXClient()
            .ConfigureHttpClient(
                client => client.BaseAddress = new Uri($"http://{options.ConneXHost.HostName}:5001/graphql"));
        
        serviceCollection.AddDbContextFactory<ConneXMetricsProviderContext, OsmoConnexMetricsProviderContextFactory>();
    }
    
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
    
    public static async Task AddConneXMetricsDatabase(this IApplicationBuilder applicationBuilder)
    {
        var dbFactory = applicationBuilder.ApplicationServices.GetRequiredService<IDbContextFactory<ConneXMetricsProviderContext>>();

        await using var connexMetricsDb = await dbFactory.CreateDbContextAsync();
     
        await connexMetricsDb.Database.MigrateAsync();
        connexMetricsDb.ApplyHypertables();
    }
}