using Microsoft.Extensions.Hosting;

namespace Osmo.ConneX.Services;

public class HandlerStatusService : IHostedService
{
    private Task _worker;
    private CancellationTokenSource _cancellationTokenSource;
    
    
    /// <summary>
    /// Triggered when the application host is ready to start the service.
    /// </summary>
    /// <param name="cancellationToken">Indicates that the start process has been aborted.</param>
    /// <returns>A <see cref="T:System.Threading.Tasks.Task" /> that represents the asynchronous Start operation.</returns>
    public Task StartAsync(CancellationToken cancellationToken)
    {
        _cancellationTokenSource = new CancellationTokenSource();
        _worker = Task.Run(DoWork, cancellationToken);
        
        return Task.CompletedTask;
    }

    /// <summary>
    /// Triggered when the application host is performing a graceful shutdown.
    /// </summary>
    /// <param name="cancellationToken">Indicates that the shutdown process should no longer be graceful.</param>
    /// <returns>A <see cref="T:System.Threading.Tasks.Task" /> that represents the asynchronous Stop operation.</returns>
    public async Task StopAsync(CancellationToken cancellationToken)
    {
        await _cancellationTokenSource.CancelAsync();
        await _worker;
        
        _cancellationTokenSource.Dispose();
    }

    private async Task DoWork()
    {
        while (!_cancellationTokenSource.Token.IsCancellationRequested)
        {
            // Do work here
            await Task.Delay(1000);
        }
    }
}