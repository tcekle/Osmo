using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Osmo.Common.Database.Models;
using Osmo.ConneX.Models;
using Osmo.ConneX.Providers;
using Osmo.Database;
using System.Threading.Channels;

namespace Osmo.ConneX.Services;

internal class ConneXRecordInjesterService : IHostedService
{
    private readonly IDbContextFactory<OsmoContext> _contextFactory;
    private readonly IDbContextFactory<ConneXMetricsProviderContext> _connexMetricsProviderContextFactory;
    private Task _worker;
    private CancellationTokenSource _cancellationTokenSource;
    // private Channel<ConneXAuditEntry> _channel = Channel.CreateUnbounded<ConneXAuditEntry>();
    private Channel<ConneXAuditEntry> _channel = Channel.CreateBounded<ConneXAuditEntry>(1000);

    public ConneXRecordInjesterService(IDbContextFactory<OsmoContext> contextFactory,
        IDbContextFactory<ConneXMetricsProviderContext> connexMetricsProviderContextFactory)
    {
        _contextFactory = contextFactory;
        _connexMetricsProviderContextFactory = connexMetricsProviderContextFactory;
    }
    
    /// <summary>
    /// Triggered when the application host is ready to start the service.
    /// </summary>
    /// <param name="cancellationToken">Indicates that the start process has been aborted.</param>
    /// <returns>A <see cref="T:System.Threading.Tasks.Task" /> that represents the asynchronous Start operation.</returns>
    public Task StartAsync(CancellationToken cancellationToken)
    {
        _cancellationTokenSource = new CancellationTokenSource();
        
        _worker = Task.Run(DoWork, _cancellationTokenSource.Token);
        
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
    
    public async Task AddRecord(ConneXAuditEntry entry)
    {
        await _channel.Writer.WriteAsync(entry);
    }
    
    private async Task DoWork()
    {
        while (!_cancellationTokenSource.Token.IsCancellationRequested)
        {
            try
            {
                var entry = await _channel.Reader.ReadAsync(_cancellationTokenSource.Token);
                await ProcessRecord(entry);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
    }
    
    private async Task ProcessRecord(ConneXAuditEntry entry)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        
        var device = await AddDeviceIfNotExists(entry, context);
        var algorithm = await AddAlgorithmIfNotExists(entry, context, device);
        var job = await AddJobIfNotExists(entry, context, algorithm);
        var programmer = await AddProgrammerIfNotExists(entry, context);
        await AddProgrammingStatistics(entry, programmer, device, job);
    }
    
    private async Task<ProgrammableDevice> AddDeviceIfNotExists(ConneXAuditEntry entry, OsmoContext context)
    {
        // Add the device if it doesn't exist
        var device = await context.ProgrammableDevices.FirstOrDefaultAsync(device => device.DeviceName == entry.Job.DeviceName);

        if (device is not null)
        {
            return device;
        }
        
        var result = context.ProgrammableDevices.Add(new ProgrammableDevice
        {
            DeviceName = entry.Job.DeviceName,
            DeviceType = entry.Job.DeviceType,
            DeviceManufacturer = entry.Job.DeviceManufacturer
        });
        
        await context.SaveChangesAsync();

        return result.Entity;
    }

    private async Task<Algorithm> AddAlgorithmIfNotExists(ConneXAuditEntry entry, OsmoContext context, ProgrammableDevice device)
    {
        // Add the algorithm if it doesn't exist
        var algorithm = await context.Algorithms.FirstOrDefaultAsync(algorithm => algorithm.AlgorithmId == entry.Job.AlgorithmId);

        if (algorithm is not null)
        {
            return algorithm;
        }

        var result = context.Algorithms.Add(new Algorithm()
        {
            AlgorithmId = entry.Job.AlgorithmId,
            AlgorithmVersion = entry.Job.AlgoVersion,
            RelatedDeviceId = device.Id
        });
        
        await context.SaveChangesAsync();

        return result.Entity;
    }
    
    private async Task<Osmo.Common.Database.Models.Job> AddJobIfNotExists(ConneXAuditEntry entry, OsmoContext context, Algorithm algorithm)
    {
        // Add the job if it doesn't exist
        var job = await context.Jobs.FirstOrDefaultAsync(job => job.GivenJobId == entry.Job.JobId);

        if (job is not null)
        {
            return job;
        }
        
        var result = context.Jobs.Add(new Osmo.Common.Database.Models.Job
        {
            GivenJobId = entry.Job.JobId,
            JobName = entry.Job.JobName,
            JobDescription = entry.Job.JobDescription,
            JobChecksum = entry.Job.JobChecksum,
            SettingChecksum = entry.Job.SettingChecksum,
            RelatedAlgorithmId = algorithm.Id
        });
        
        await context.SaveChangesAsync();

        return result.Entity;
    }
    
    private async Task<LumenXProgrammer> AddProgrammerIfNotExists(ConneXAuditEntry entry, OsmoContext context)
    {
        // Add the programmer if it doesn't exist
        var programmer = await context.LumenXProgrammers.FirstOrDefaultAsync(programmer => programmer.UniqueIdentifier == entry.Programmer.SerialNumber);

        if (programmer is not null)
        {
            return programmer;
        }
        
        var result = context.LumenXProgrammers.Add(new LumenXProgrammer
        {
            UniqueIdentifier = entry.Programmer.SerialNumber,
            FirmwareVersion = entry.Programmer.FirmwareVersion,
            SystemVersion = entry.Programmer.SystemVersion
        });
        
        await context.SaveChangesAsync();

        return result.Entity;
    }
    
    private async Task AddProgrammingStatistics(ConneXAuditEntry entry, 
        LumenXProgrammer programmer,
        ProgrammableDevice device,
        Osmo.Common.Database.Models.Job job)
    {
        await using var connexMetricsContext = await _connexMetricsProviderContextFactory.CreateDbContextAsync();
        
        // Add the programming statistics
        connexMetricsContext.ProgrammingStatistics.Add(new ProgrammingStatistic
        {
            RelatedDeviceId = device.Id,
            RelatedJobId = job.Id,
            RelatedProgrammerId = programmer.Id,
            Code = int.Parse(entry.PartDetail.Result.Code ?? "0"),
            CodeName = entry.PartDetail.Result.CodeName,
            EraseDuration = int.Parse(entry.PartDetail.Result.EraseDuration ?? "0"),
            ProgramDuration = entry.PartDetail.Result.ProgramDuration,
            VerifyDuration = entry.PartDetail.Result.VerifyDuration,
            BlankCheckDuration = int.Parse(entry.PartDetail.Result.BlankCheckDuration ?? "0"),
            BytesProgrammed = entry.PartDetail.Result.BytesProgrammed,
            Overall = int.Parse(entry.PartDetail.Result.Overall ?? "0"),
            Overhead = int.Parse(entry.PartDetail.Result.Overhead ?? "0"),
            TimeStamp = entry.TimeStamp.ToUniversalTime()
        });
        
        await connexMetricsContext.SaveChangesAsync();
    }
}