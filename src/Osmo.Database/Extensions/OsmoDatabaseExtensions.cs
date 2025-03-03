using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Osmo.Database.Extensions;

/// <summary>
/// Extension methods for adding Osmo database to the application.
/// </summary>
public static class OsmoDatabaseExtensions
{
    /// <summary>
    /// Adds Osmo database to the application.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to add the services to.</param>
    public static void AddOsmoDatabase(this IServiceCollection services)
    {
        services.AddDbContextFactory<OsmoContext, OsmoContextFactory>();
    }

    /// <summary>
    /// Configures Osmo database.
    /// </summary>
    /// <param name="applicationBuilder"></param>
    public static async Task ConfigureOsmoDatabase(this IApplicationBuilder applicationBuilder)
    {
        var dbFactory = applicationBuilder.ApplicationServices.GetRequiredService<IDbContextFactory<OsmoContext>>();

        await using var osmoDb = await dbFactory.CreateDbContextAsync();
     
        await osmoDb.Database.MigrateAsync();
    }
}