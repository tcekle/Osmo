using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Options;

namespace Osmo.Database;

using Common.Database.Options;

/// <summary>
/// Osmo context factory
/// </summary>
public class OsmoContextFactory :  IDbContextFactory<OsmoContext>, IDesignTimeDbContextFactory<OsmoContext>
{
    private readonly IOptions<PostgresOptions> _databaseOptions;

    /// <summary>
    /// Creates a new instance of <see cref="OsmoContextFactory"/>
    /// </summary>
    public OsmoContextFactory()
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
    /// Creates a new instance of <see cref="OsmoContextFactory"/>
    /// </summary>
    /// <param name="databaseOptions">An instance of <see cref="IOptions{TOptions}"/> for <see cref="PostgresOptions"/></param>
    public OsmoContextFactory(IOptions<PostgresOptions> databaseOptions)
    {
        _databaseOptions = databaseOptions;
    }
    
    /// <summary>Creates a new instance of a derived context.</summary>
    /// <param name="args">Arguments provided by the design-time service.</param>
    /// <returns>An instance of <typeparamref name="TContext" />.</returns>
    public OsmoContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<OsmoContext>();
        optionsBuilder.UseNpgsql($"Host={_databaseOptions.Value.Host};Database=osmo;Username={_databaseOptions.Value.Username};Password={_databaseOptions.Value.Password};Port={_databaseOptions.Value.Port}");
                
        return new OsmoContext(optionsBuilder.Options);
    }

    /// <summary>
    ///     Creates a new <see cref="T:Microsoft.EntityFrameworkCore.DbContext" /> instance.
    /// </summary>
    /// <remarks>
    ///     The caller is responsible for disposing the context; it will not be disposed by any dependency injection container.
    /// </remarks>
    /// <returns>A new context instance.</returns>
    public OsmoContext CreateDbContext() => CreateDbContext([]);
}