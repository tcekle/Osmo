using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Osmo.Database;

/// <summary>
/// Osmo context factory
/// </summary>
public class OsmoContextFactory :  IDbContextFactory<OsmoContext>, IDesignTimeDbContextFactory<OsmoContext>
{
    /// <summary>Creates a new instance of a derived context.</summary>
    /// <param name="args">Arguments provided by the design-time service.</param>
    /// <returns>An instance of <typeparamref name="TContext" />.</returns>
    public OsmoContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<OsmoContext>();
        optionsBuilder.UseNpgsql(OsmoContext.DefaultConnectionString);
                
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