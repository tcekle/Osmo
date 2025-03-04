using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Osmo.Database.Extensions;

using Common.Database.Options;

/// <summary>
/// Extension methods for adding Osmo database to the application.
/// </summary>
public static class OsmoDatabaseExtensions
{
    /// <summary>
    /// Adds Osmo database to the application.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to add the services to.</param>
    public static void AddOsmoDatabase(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContextFactory<OsmoContext, OsmoContextFactory>();
        
        services.Configure<PostgresOptions>(configuration.GetSection(nameof(PostgresOptions)).Bind);
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