using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Containers;
using DotNet.Testcontainers.Networks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace Osmo.Test;

using Osmo.Common.Database.Options;

internal class OsmoServiceFactory : WebApplicationFactory<Program>
{
    private IContainer? _dbContainer;
    private static HashSet<int> _ports = new HashSet<int>();
    private int _port;
    private static readonly object _portLocker = new object();

    /// <summary>
    /// Gets the database container
    /// </summary>
    public IContainer? DbContainer { get => _dbContainer; }
    
    /// <summary>
    /// Creates a new instance of <see cref="OsmoServiceFactory"/>
    /// </summary>
    public OsmoServiceFactory()
    {
        lock (_portLocker)
        {
            _port = Random.Shared.Next(10_000, 60_000);
            while (_ports.Contains(_port))
            {
                _port = _port = new Random().Next(10000, 60000);
            }

            _ports.Add(_port);
        }
    }
    
    public async Task Initialize()
    {
        INetwork network;// = new DockerNetwork(Guid.NewGuid().ToString(), nameof(ConneXServiceFactory));

        network = new NetworkBuilder()
            .WithName(Guid.NewGuid().ToString("D"))
            .Build();
            
        _dbContainer = new ContainerBuilder()
            .WithImage("timescale/timescaledb:latest-pg14")
            .WithEnvironment("POSTGRES_USER", "dataio")
            .WithEnvironment("POSTGRES_PASSWORD", "dataio")
            .WithPortBinding(_port, 5432)
            .WithWaitStrategy(Wait.ForUnixContainer().UntilPortIsAvailable(5432))
            .WithNetwork(network)
            .Build();

        await network.CreateAsync();
    }
    
    /// <summary>
    /// Gives a fixture an opportunity to configure the application before it gets built.
    /// </summary>
    /// <param name="builder">The <see cref="T:Microsoft.AspNetCore.Hosting.IWebHostBuilder" /> for the application.</param>
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
            
        builder.ConfigureTestServices(services =>
        {
            var logger = LoggerFactory.Create(config =>
            {
                config.SetMinimumLevel(LogLevel.Debug);
                config.AddDebug();
                config.AddConsole();
            }).CreateLogger("TestContainers");
                
            services.AddSingleton<ILoggerFactory>(s => new NullLoggerFactory());
                
            services.Configure<PostgresOptions>(options =>
            {
                options.Host = "127.0.0.1";
                options.Username = "dataio";
                options.Password = "dataio";
                options.Port = _port;
            });
        });
    }
}