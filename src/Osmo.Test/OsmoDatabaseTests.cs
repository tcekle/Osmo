using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Osmo.Test;

using Database;

public class OsmoDatabaseTests
{
    private static OsmoServiceFactory _osmoServiceFactory = new();
    
    [Before(Class)]
    public static async Task Initialize()
    {
        await _osmoServiceFactory.Initialize();

        if (_osmoServiceFactory.DbContainer is null)
        {
            Assert.Fail("DbContainer is null");;
        }
        
        await _osmoServiceFactory.DbContainer.StartAsync();
    }
    
    public static async Task Cleanup()
    {
        await _osmoServiceFactory.DisposeAsync();

        if (_osmoServiceFactory.DbContainer is not null)
        {
            await _osmoServiceFactory.DbContainer.StopAsync();
        }
    }
    
    [Test]
    public async Task OsmoContext_CanBeCreated()
    {
        var dbContextFactory = _osmoServiceFactory.Services.GetRequiredService<IDbContextFactory<OsmoContext>>();
        
        await using var dbContext = await dbContextFactory.CreateDbContextAsync();

        await Assert.That(dbContext).IsNotNull(); 
    }
}