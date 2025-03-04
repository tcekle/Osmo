using System.Diagnostics;
using Osmo;
using Serilog;
using Serilog.Events;

IConfigurationRoot baseConfiguration;
string appSettingsPath = Path.Combine(Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName), "appsettings.json");
if (File.Exists(appSettingsPath))
{
    baseConfiguration = new ConfigurationBuilder()
        .AddJsonFile(appSettingsPath)
        .Build();
}
else
{
    baseConfiguration = new ConfigurationBuilder().Build();
}


// The initial "bootstrap" logger is able to log errors during start-up. It's completely replaced by the
// logger configured in `UseSerilog()` below, once configuration and dependency-injection have both been set up successfully.
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
    .ReadFrom.Configuration(baseConfiguration)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .CreateLogger();


var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((context, serviceProvider, configuration) =>
{
    configuration.ReadFrom.Configuration(baseConfiguration)
        .Enrich.FromLogContext()
        .WriteTo.Console();
});

// Add services to the container.
builder.Services.AddRazorPages();

builder.Services.AddOsmoServices(baseConfiguration);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseStaticFiles();

app.UseRouting();

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

await app.ConfigureOsmoServices();

await app.RunAsync();