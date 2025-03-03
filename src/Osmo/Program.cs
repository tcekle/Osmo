using System.Diagnostics;
using System.Reflection;
using MassTransit;
using Microsoft.AspNetCore.Components.Routing;
using Microsoft.AspNetCore.Components.Web;
using MudBlazor.Services;
using Osmo;
using Osmo.Common.Extensions;
using Osmo.Common.Plugins;
using Osmo.Common.Services;
using Osmo.ConneX.Extensions;
using Osmo.Data;
using Osmo.Pages;
using Osmo.Services;
using Serilog;
using Serilog.Events;



var configuration = new ConfigurationBuilder()
    .AddJsonFile(Path.Combine(Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName), "appsettings.json"))
    .Build();


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();

builder.Services.AddOsmoServices(configuration);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

// app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

await app.ConfigureOsmoServices();

// var plugins = app.Services.GetRequiredService<IEnumerable<IOsmoPlugin>>();
// foreach (var plugin in plugins)
// {
//     app.Map
// }
//Router router = app.Services.GetRequiredService<Router>(); 

app.Run();