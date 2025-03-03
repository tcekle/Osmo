using Microsoft.EntityFrameworkCore;
using Osmo.Common.Database.Models;

namespace Osmo.Database;

/// <summary>
/// Context for the Osmo database
/// </summary>
public class OsmoContext : DbContext
{
    internal const string DefaultConnectionString = "Host=127.0.0.1;Database=osmo;Username=dataio;Password=dataio";
    private static DbContextOptions _defaultOptions = new DbContextOptionsBuilder().UseNpgsql(DefaultConnectionString).Options;
    
    /// <summary>
    /// Gets or sets the <see cref="DbSet{TEntity}"/> of <see cref="ProgrammableDevice"/>s
    /// </summary>
    public DbSet<ProgrammableDevice> ProgrammableDevices { get; set; }
    
    /// <summary>
    /// Gets or sets the <see cref="DbSet{TEntity}"/> of <see cref="Algorithm"/>s
    /// </summary>
    public DbSet<Algorithm> Algorithms { get; set; }
    
    /// <summary>
    /// Gets or sets the <see cref="DbSet{TEntity}"/> of <see cref="Job"/>s
    /// </summary>
    public DbSet<Job> Jobs { get; set; }
    
    /// <summary>
    /// Gets or sets the <see cref="DbSet{TEntity}"/> of <see cref="LumenXProgrammer"/>s
    /// </summary>
    public DbSet<LumenXProgrammer> LumenXProgrammers { get; set; }
    
    /// <summary>
    /// Creates a new instance of <see cref="OsmoContext"/> with the default options
    /// Create a new migration like so:
    ///   dotnet ef migrations add MIGRATION_NAME --startup-project Osmo.Database.csproj --context OsmoContext --output-dir Migrations/Osmo
    /// </summary>
    internal OsmoContext()
        : base(_defaultOptions)
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