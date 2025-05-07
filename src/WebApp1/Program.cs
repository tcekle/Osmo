using Osmo;
using Osmo.ConneX.Extensions;
using Osmo.Database.Extensions;
using System.Diagnostics;

var configurationBuilder = new ConfigurationBuilder();
string appSettingsPath = Path.Combine(Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName), "appsettings.json");
string devAppSettingsPath = Path.Combine(Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName), "appsettings.Development.json");

if (File.Exists(appSettingsPath))
{
    configurationBuilder.AddJsonFile(appSettingsPath);
}

if (File.Exists(devAppSettingsPath))
{
    configurationBuilder.AddJsonFile(devAppSettingsPath);
}

var baseConfiguration = configurationBuilder.Build();

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllersWithViews();
builder.Services.AddOsmoServices(baseConfiguration);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();


app.MapControllerRoute(
    name: "default",
    pattern: "{controller}/{action=Index}/{id?}");

app.MapFallbackToFile("index.html");

await app.ConfigureOsmoDatabase();


app.UseEndpoints(endpoints =>
{
    endpoints.UseGraphQlEndpoints();
});

await app.RunAsync();