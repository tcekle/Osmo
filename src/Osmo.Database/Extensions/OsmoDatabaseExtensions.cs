using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Osmo.Database.Extensions;

public static class OsmoDatabaseExtensions
{
    public static void AddOsmoDatabase(this IServiceCollection services)
    {
        services.AddDbContextFactory<OsmoContext, OsmoContextFactory>();
    }

    public static async Task AddOsmoDatabase(this IApplicationBuilder applicationBuilder)
    {
        var dbFactory = applicationBuilder.ApplicationServices.GetRequiredService<IDbContextFactory<OsmoContext>>();

        await using var osmoDb = await dbFactory.CreateDbContextAsync();
     
        await osmoDb.Database.MigrateAsync();
    }
}