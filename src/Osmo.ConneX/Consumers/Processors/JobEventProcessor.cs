using Dapper;
using Microsoft.EntityFrameworkCore;
using Osmo.Common.Messages;
using Osmo.ConneX.Models;
using Osmo.ConneX.Providers;
using Osmo.Database;
using System.Text.Json;

namespace Osmo.ConneX.Consumers.Processors;

internal class JobEventProcessor
{
    internal const string ZERO_BYTE_PROGRAM_SUCCESS = "ZeroByteProgramSuccess";
    internal const string PROGRAM_SKIPPED = "ProgramSkipped";
    internal const string HANDLER_SKIPPED_SOCKET = "HandlerSkippedSocket";
    internal const string ADAPTER_ACTUATION_LIMIT_EXCEEDED = "AdapterActuationLimitExceeded";
    internal const string PROGRAM_DURATION_DEVIATION = "ProgramDurationDeviation";
    
    private readonly IDbContextFactory<ConneXMetricsProviderContext> _connexMetricsProviderContextFactory;
    private readonly IDbContextFactory<OsmoContext> _osmoContextFactory;

    private static readonly Func<ConneXMetricsProviderContext, Guid, DateTime, Task<bool>> DoesGuidExist =
        EF.CompileAsyncQuery((ConneXMetricsProviderContext context, Guid id, DateTime minDate) =>
            context.JobEvents
                .AsNoTracking()
                .Any(e => e.RelatedMessageId == id && e.Timestamp >= minDate));
    
    private static readonly Func<OsmoContext, Guid, Task<Common.Database.Models.Job>> _getJobByGivenId =
        EF.CompileAsyncQuery((OsmoContext context, Guid jobId) =>
            context.Jobs.AsNoTracking().FirstOrDefault(j => j.GivenJobId == jobId));
    
    // private static readonly Func<ConneXMetricsProviderContext, Guid, DateTime, Task<bool>> DoesHandlerStatisticExist =
    //     EF.CompileAsyncQuery((ConneXMetricsProviderContext context, Guid id, DateTime minDate) =>
    //         context.HandlerStatistics
    //             .AsNoTracking()
    //             .Any(e => e.RelatedMessageId == id && e.TimeStamp >= minDate));

    private List<Func<ConneXAuditEntry, 
        MqttMessage, 
        ConneXMetricsProviderContext,
        Common.Database.Models.Job,
        Task>> _eventProcessors;
    
    
    public JobEventProcessor(IDbContextFactory<ConneXMetricsProviderContext> connexMetricsProviderContextFactory,
                                IDbContextFactory<OsmoContext> osmoContextFactory)
    {
        _connexMetricsProviderContextFactory = connexMetricsProviderContextFactory;
        _osmoContextFactory = osmoContextFactory;

        _eventProcessors =
        [
            IsZeroByteSuccess,
            IsProgramSkipped,
            IsHandlerSkippedSocket,
            IsAdapterActuationLimitExceeded,
            CheckForProgrammingAnomalies
        ];
    }
    
    public async Task ProcessEvent(MqttMessage message)
    {
        await using var context = await _connexMetricsProviderContextFactory.CreateDbContextAsync();

        if (await DoesGuidExist(context, message.Id, message.Timestamp.ToUniversalTime().AddDays(-1)))
        {
            return;
        }
        
        
        
        
        Task processor = message.Topic switch
        {
            "connex/programmer/lumenx/legacy/programmingcomplete" => ExtractEvents(message, context),
            _ => Task.CompletedTask
        };

        await processor;
    }
    
    private async Task ExtractEvents(MqttMessage message, ConneXMetricsProviderContext context)
    {
        var records = DeserializePayload(message.PayloadAsString);
        
        if (records == null || records.Count == 0)
        {
            return;
        }
        
        await using var osmoContext = await _osmoContextFactory.CreateDbContextAsync();
        // var job = await osmoContext.Jobs.FirstOrDefaultAsync(j => j.GivenJobId == records[0].Job.JobId);
        var job = await _getJobByGivenId(osmoContext, records[0].Job.JobId);
        
        foreach (var record in records)
        {
            foreach (var eventProcessor in _eventProcessors)
            {
                await eventProcessor(record, message, context, job);
            }
        }

        
        // string[] levels = message.Topic.Split("/");
        //
        // return levels[2] switch
        // {
        //     "devicecomplete" => ParseAhOperationDeviceComplete(message),
        //     "pick" => ParseAhPick(message),
        //     "place" => ParseAhPlace(message),
        //     _ => Task.CompletedTask
        // };
    }
    
    private async Task IsZeroByteSuccess(ConneXAuditEntry entry, MqttMessage message, ConneXMetricsProviderContext context, Common.Database.Models.Job job)
    {
        if (entry?.PartDetail?.Result == null)
        {
            return;
        }

        var result = entry.PartDetail.Result;
        if (result.Code == "0" && result.BytesProgrammed == 0)
        {
            await AddJobEventToDatabase(new JobEvent
            {
                Type = ZERO_BYTE_PROGRAM_SUCCESS,
                Title = "Zero byte program success",
                Message =
                    $"Socket {entry.PartDetail.Result.SocketIndex} completed programming with 0 bytes written. Job: {entry.Job.JobName}",
                Timestamp = message.Timestamp,
                RelatedMessageId = message.Id,
                JobIdentifier = job.Id.ToString()
            },
            context);
        }
    }
    
    private async Task IsProgramSkipped(ConneXAuditEntry entry, MqttMessage message, ConneXMetricsProviderContext context, Common.Database.Models.Job job)
    {
        var result = entry?.PartDetail?.Result;
        if (result == null)
        {
            return;
        }

        if (result.Code == "0" &&
            result.BytesProgrammed == 0 &&
            result.ProgramDuration == 0 &&
            result.VerifyDuration == 0)
        {
            await AddJobEventToDatabase(new JobEvent
            {
                Type = PROGRAM_SKIPPED,
                Title = "Programming was skipped",
                Message = $"Programming was skipped for socket {result.SocketIndex}. Job: {entry.Job.JobName}",
                Timestamp = message.Timestamp,
                RelatedMessageId = message.Id,
                JobIdentifier = job.Id.ToString()
            },
            context);
        }
    }
    
    private async Task IsHandlerSkippedSocket(ConneXAuditEntry entry, MqttMessage message, ConneXMetricsProviderContext context, Common.Database.Models.Job job)
    {
        var result = entry?.PartDetail?.Result;
        if (result == null)
        {
            return;
        }

        bool allDurationsZero =
            result.ProgramDuration == 0 &&
            result.VerifyDuration == 0 &&
            result.BytesProgrammed == 0;

        if (result.Code == "0" && allDurationsZero)
        {
            await AddJobEventToDatabase(new JobEvent
            {
                Type = HANDLER_SKIPPED_SOCKET,
                Title = "Handler may have skipped socket",
                Message = $"No activity recorded on socket {result.SocketIndex} — possible no device present. Job: {entry.Job.JobName}",
                Timestamp = message.Timestamp,
                RelatedMessageId = message.Id,
                JobIdentifier = job.Id.ToString()
            },
            context);
        }
    }
    
    private async Task IsAdapterActuationLimitExceeded(ConneXAuditEntry entry, MqttMessage message, ConneXMetricsProviderContext context, Common.Database.Models.Job job)
    {
        // You may need to enable this if the Adapter is nested in Programmer
        var adapter = entry?.Programmer?.Adapter;
        if (adapter == null)
        {
            return;
        }

        string adapterId = adapter.AdapterId;
        int actualCount = int.Parse(adapter.LifetimeActuationCount);
        int recommendedLimit = adapterId[0] switch
        {
            '1' => 5_000,
            '2' => 50_000,
            '3'=> 200_000,
            '4' => 5_000,
            _ => 0
        };

        if (recommendedLimit == 0)
        {
            return; // Unrecognized adapter type
        }

        if (actualCount > recommendedLimit)
        {
            await AddJobEventToDatabase(new JobEvent
            {
                Type = ADAPTER_ACTUATION_LIMIT_EXCEEDED,
                Title = "Adapter Actuation Limit Exceeded",
                Message = $"Programming occured with an adapter that exceeded recommended limit. Adapter {adapterId} (Socket {adapter.SocketIndex}) has {actualCount} actuations, exceeding the recommended limit of {recommendedLimit}.",
                Timestamp = message.Timestamp,
                RelatedMessageId = message.Id,
                JobIdentifier = job.Id.ToString()
            },
            context);
        }
    }

    private async Task CheckForProgrammingAnomalies(ConneXAuditEntry entry, MqttMessage message, ConneXMetricsProviderContext context, Common.Database.Models.Job job)
    {
        if (job is null || entry?.Job?.JobId == Guid.Empty || entry.PartDetail?.Result == null)
        {
            return;
        }

        var jobId = job.Id;
        var latestDuration = entry.PartDetail.Result.ProgramDuration;

        
        /*
         * CREATE MATERIALIZED VIEW avg_program_duration_per_job
WITH (timescaledb.continuous) AS
SELECT
  related_job_id,
  time_bucket('1 hour', timestamp) AS bucket,
  AVG(program_duration) AS avg_duration
FROM programming_statistics
GROUP BY related_job_id, bucket;
         */
        
    //     const string sql = @"
    //     WITH latest_entry AS (
    //         SELECT timestamp, program_duration
    //         FROM programming_statistics
    //         WHERE related_job_id = @JobId
    //         ORDER BY timestamp DESC
    //         LIMIT 1
    //     ),
    //     last_100 AS (
    //         SELECT program_duration
    //         FROM programming_statistics
    //         WHERE related_job_id = @JobId
    //         AND timestamp < (SELECT timestamp FROM latest_entry)
    //         ORDER BY timestamp DESC
    //         LIMIT 100
    //     )
    //     SELECT
    //         (SELECT AVG(program_duration) FROM last_100) AS avg_last_100,
    //         (SELECT program_duration FROM latest_entry) AS latest_program_duration;
    // ";

        string sql = """
                     SELECT avg_duration
                     FROM avg_program_duration_per_job
                     WHERE related_job_id = @JobId
                     ORDER BY bucket DESC
                     LIMIT 1;
                     """;

        var result = await context.Database.GetDbConnection().QueryFirstOrDefaultAsync<double>(
            sql,
            new { JobId = jobId });

        if (result == default)
            return;

        double lowerBound = result * 0.90;
        double upperBound = result * 1.10;

        if (latestDuration < lowerBound || latestDuration > upperBound)
        {
            await AddJobEventToDatabase(new JobEvent
            {
                Type = PROGRAM_DURATION_DEVIATION,
                Title = "Program Duration Deviation Detected",
                Message =
                    $"Program duration {latestDuration}ms is outside ±10% of the recent average ({result:F2}ms).",
                Timestamp = message.Timestamp,
                RelatedMessageId = message.Id,
                JobIdentifier = job.Id.ToString()
            },
            context);
        }
    }

    private async Task AddJobEventToDatabase(JobEvent jobEvent, ConneXMetricsProviderContext context)
    {
        // await using var context = await _connexMetricsProviderContextFactory.CreateDbContextAsync();
        await context.JobEvents.AddAsync(jobEvent);
        await context.SaveChangesAsync();
    }
    
    private static List<ConneXAuditEntry> DeserializePayload(string payload)
    {
        JsonSerializerOptions jsonSerializerOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };
        
        try
        {
            return JsonSerializer.Deserialize<List<ConneXAuditEntry>>(payload, jsonSerializerOptions);
        }
        catch (Exception)
        {
            // If the payload is not an array, try to deserialize it as a single object.
        }

        return [JsonSerializer.Deserialize<ConneXAuditEntry>(payload, jsonSerializerOptions)];
    }
}