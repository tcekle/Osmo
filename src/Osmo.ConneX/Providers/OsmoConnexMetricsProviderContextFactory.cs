using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Options;

namespace Osmo.ConneX.Providers;

using Common.Database.Options;

/// <summary>
/// Context factory for the ConneX metrics provider.
/// </summary>
internal class OsmoConnexMetricsProviderContextFactory : IDesignTimeDbContextFactory<ConneXMetricsProviderContext>, IDbContextFactory<ConneXMetricsProviderContext>
{
    private readonly IOptions<PostgresOptions> _databaseOptions;

    /// <summary>
    /// Creates a new instance of the <see cref="OsmoConnexMetricsProviderContextFactory"/> class.
    /// </summary>
    public OsmoConnexMetricsProviderContextFactory()
    {
        _databaseOptions = Options.Create<PostgresOptions>(new PostgresOptions()
        {
            Host = "127.0.0.1",
            Port = 5432,
            Username = "dataio",
            Password = "dataio"
        });
    }
    
    /// <summary>
    /// Creates a new instance of the <see cref="OsmoConnexMetricsProviderContextFactory"/> class.
    /// </summary>
    /// <param name="databaseOptions">The <see cref="IOptions{T}"/> instance for <see cref="PostgresOptions"/> to use for the database connection.</param>
    public OsmoConnexMetricsProviderContextFactory(IOptions<PostgresOptions> databaseOptions)
    {
        _databaseOptions = databaseOptions;
    }
    
    /// <summary>
    /// Create a new instance of the <see cref="ConneXMetricsProviderContext"/> class.
    /// </summary>
    /// <returns></returns>
    public ConneXMetricsProviderContext CreateDbContext() => CreateDbContext([]);

    /// <summary>Creates a new instance of a derived context.</summary>
    /// <param name="args">Arguments provided by the design-time service.</param>
    /// <returns>An instance of <typeparamref name="TContext" />.</returns>
    public ConneXMetricsProviderContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<ConneXMetricsProviderContext>();
        optionsBuilder.UseNpgsql($"Host={_databaseOptions.Value.Host};Database=osmo_connex_metrics;Username={_databaseOptions.Value.Username};Password={_databaseOptions.Value.Password};Port={_databaseOptions.Value.Port}");
                
        return new ConneXMetricsProviderContext(optionsBuilder.Options);
    }
}