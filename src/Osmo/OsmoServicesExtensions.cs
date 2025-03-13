using MassTransit;
using Microsoft.AspNetCore.Components.Server;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.EntityFrameworkCore;
using MudBlazor.Services;
using Osmo.Common.Extensions;
using Osmo.Common.Services;
using Osmo.Common.Ui;
using Osmo.Components;
using Osmo.ConneX.Extensions;
using Osmo.ConneX.Ui;
using Osmo.ConneX.Ui.MessageViewer;
using Osmo.Data;
using Osmo.Database;
using Osmo.Database.Extensions;
using Osmo.Services;
using Serilog;
using System.Reflection;

namespace Osmo;

public static class OsmoServicesExtensions
{
    public static void AddOsmoServices(this IServiceCollection services, IConfiguration configuration)
    {
        Version assemblyVersion = Assembly.GetEntryAssembly()?.GetName().Version;
        
        services.AddServerSideBlazor(options =>
        {
            // options.RootComponents.RegisterForJavaScript<Counter>("counter");
            options.AddBuiltInComponents();
            options.AddRootComponets(ConneXPluginExtensions.GetConneXRootComponents());
        });

        services.AddSingleton<WeatherForecastService>();

        services.AddMudServices();
        services.AddOsmoDatabase(configuration);

        services.AddSerilog(config =>
        {
            config
                .ReadFrom.Configuration(configuration)
                .Enrich.FromLogContext()
                .WriteTo.Console()
                .Enrich.WithProperty("version", assemblyVersion.ToString())
                .Enrich.WithProperty("app", "OSMO")
                .Enrich.WithProperty("hostname", System.Environment.MachineName);
        });

        services.AddMassTransit(busConfig =>
        {
            busConfig.AddConsumer<MassTransitNotificationMessageConsumer>();
            busConfig.AddConsumers(typeof(Program).Assembly);
            busConfig.AddConneXBusConsumers();
            busConfig.UsingRabbitMq((context, cfg) =>
            {
                cfg.Host("localhost", "/", h =>
                {
                    h.Username("admin");
                    h.Password("admin");
                });

                cfg.AutoStart = true;
                cfg.ConfigureEndpoints(context);
            });
        });
        
        services.AddConneXPlugin(configuration);

        services.AddSingleton<GlobalNotificationService>();
        services.AddScoped<GoldenLayoutService>();
        services.AddScoped<MainLayoutLeftToolbarService>();
        services.AddSingleton<StatusBarService>();
        services.AddSingleton<IStatusBar>(s => s.GetRequiredService<StatusBarService>());
    }

    public static async Task ConfigureOsmoServices(this IApplicationBuilder applicationBuilder)
    {
        await applicationBuilder.ConfigureOsmoDatabase();
        await applicationBuilder.ConfigureConneXMetricsDatabase();
    }
    
    private static void AddBuiltInComponents(this CircuitOptions options)
    {
        // options.RootComponents.RegisterForJavaScript<DeviceList>("osmo-device-list");
    }

    private static void AddRootComponets(this CircuitOptions options, IEnumerable<LayoutComponentRegistration> componentRegistrations)
    {
        foreach (var componentRegistration in componentRegistrations)
        {
            options.RootComponents.RegisterForJavaScript(componentRegistration.ComponentType, componentRegistration.ComponentIdentifier);
        }
    }
}