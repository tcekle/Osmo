using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Osmo.Common.Database.Models;

namespace Osmo.Database;

public class OsmoContext : DbContext
{
    public static string DefaultConnectionString = $"Host=127.0.0.1;Database=osmo;Username=dataio;Password=dataio";
    public static DbContextOptions DefaultOptions = new DbContextOptionsBuilder().UseNpgsql(DefaultConnectionString).Options;
    
    public DbSet<ProgrammableDevice> ProgrammableDevices { get; set; }
    public DbSet<Algorithm> Algorithms { get; set; }
    public DbSet<Job> Jobs { get; set; }
    
    public DbSet<LumenXProgrammer> LumenXProgrammers { get; set; }
    
    /// <summary>
    /// Creates a new instance of <see cref="OsmoContext"/> with the default options
    /// Create a new migration like so:
    ///   dotnet ef migrations add MIGRATION_NAME --startup-project Osmo.Database.csproj --context OsmoContext --output-dir Migrations/Osmo
    /// </summary>
    internal OsmoContext()
        : base(DefaultOptions)
    {
    }
    
    /// <summary>
    /// Creates a new instance of <see cref="OsmoContext"/>
    /// </summary>
    /// <param name="options">The <see cref="DbContextOptions"/> to use</param>
    public OsmoContext(DbContextOptions options)
        : base(options)
    {

    }
}

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