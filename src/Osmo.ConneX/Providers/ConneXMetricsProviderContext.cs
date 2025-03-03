using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Osmo.ConneX.Models;

namespace Osmo.ConneX.Providers;

internal class ConneXMetricsProviderContext : DbContext
{
    public static string DefaultConnectionString = $"Host=127.0.0.1;Database=osmo_connex_metrics;Username=dataio;Password=dataio";
    public static DbContextOptions DefaultOptions = new DbContextOptionsBuilder().UseNpgsql(DefaultConnectionString).Options;
    
    public DbSet<ProgrammingStatistic> ProgrammingStatistics { get; set; }
    
    /// <summary>
    ///
    /// Create a new migration like so:
    ///   dotnet ef migrations add MIGRATION_NAME --startup-project Osmo.ConneX.csproj --context ConneXMetricsProviderContext --output-dir Migrations/ConneXMetrics
    /// </summary>
    /// </summary>
    /// <param name="options"></param>
    public ConneXMetricsProviderContext(DbContextOptions options) 
        : base(options)
    {
    }
    
    public ConneXMetricsProviderContext()
        : base(DefaultOptions)
    {
    }

    /// <summary>
    ///     Override this method to further configure the model that was discovered by convention from the entity types
    ///     exposed in <see cref="T:Microsoft.EntityFrameworkCore.DbSet`1" /> properties on your derived context. The resulting model may be cached
    ///     and re-used for subsequent instances of your derived context.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         If a model is explicitly set on the options for this context (via <see cref="M:Microsoft.EntityFrameworkCore.DbContextOptionsBuilder.UseModel(Microsoft.EntityFrameworkCore.Metadata.IModel)" />)
    ///         then this method will not be run. However, it will still run when creating a compiled model.
    ///     </para>
    ///     <para>
    ///         See <see href="https://aka.ms/efcore-docs-modeling">Modeling entity types and relationships</see> for more information and
    ///         examples.
    ///     </para>
    /// </remarks>
    /// <param name="modelBuilder">
    ///     The builder being used to construct the model for this context. Databases (and other extensions) typically
    ///     define extension methods on this object that allow you to configure aspects of the model that are specific
    ///     to a given database.
    /// </param>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ProgrammingStatistic>().HasKey(table => new { table.Id, table.TimeStamp });
    }
}

internal class OsmoConnexMetricsProviderContextFactory : IDesignTimeDbContextFactory<ConneXMetricsProviderContext>, IDbContextFactory<ConneXMetricsProviderContext>
{
    public ConneXMetricsProviderContext CreateDbContext() => CreateDbContext([]);

    /// <summary>Creates a new instance of a derived context.</summary>
    /// <param name="args">Arguments provided by the design-time service.</param>
    /// <returns>An instance of <typeparamref name="TContext" />.</returns>
    public ConneXMetricsProviderContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<ConneXMetricsProviderContext>();
        optionsBuilder.UseNpgsql(ConneXMetricsProviderContext.DefaultConnectionString);
                
        return new ConneXMetricsProviderContext(optionsBuilder.Options);
    }
}