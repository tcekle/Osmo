using HotChocolate.AspNetCore;
using HotChocolate.Execution.Configuration;
using HotChocolate.Types.Pagination;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Osmo.Common.Database.Models;
using Osmo.Database.Graphql.Query;
using Osmo.Database.Graphql.Types;

namespace Osmo.Database.Extensions;

using Common.Database.Options;

/// <summary>
/// Extension methods for adding Osmo database to the application.
/// </summary>
public static class OsmoDatabaseExtensions
{
    public const string ROOT_QUERY_NAME = "Query";
    
    /// <summary>
    /// Adds Osmo database to the application.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to add the services to.</param>
    public static void AddOsmoDatabase(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContextFactory<OsmoContext, OsmoContextFactory>();
        services.AddTransient<OsmoContext>(s => s.GetRequiredService<IDbContextFactory<OsmoContext>>().CreateDbContext());
        
        services.Configure<PostgresOptions>(configuration.GetSection(nameof(PostgresOptions)).Bind);

        // services.AddGraphQLServer("Default")
        //     .AddQueryType<JobQuery>()
        //     .AddPagingArguments()
        //     .AddFiltering()
        //     .AddSorting()
        //     .AddProjections();
        // .InitializeOnStartup()
        // // .AddQueryType<Query>()
        // .AddType<Job>()
        // .AddFiltering()
        // .AddSorting()
        // .AddProjections();
    }
    
    public static IRequestExecutorBuilder AddOsmoGraphQL(this IRequestExecutorBuilder builder)
    {
        builder.AddTypeExtension<JobQuery>();
        // builder.AddQueryType<JobQuery>();

        return builder;
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