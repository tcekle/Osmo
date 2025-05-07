using Dapper;
using GreenDonut.Data;
using HotChocolate;
using HotChocolate.Types;
using Microsoft.EntityFrameworkCore;
using Osmo.ConneX.Models;
using Osmo.ConneX.Providers;
using Osmo.Database;
using Osmo.Database.Extensions;

namespace Osmo.ConneX.GraphQl.Queries;

[ExtendObjectType(OsmoDatabaseExtensions.ROOT_QUERY_NAME)]
internal class JobStatistics
{
    public async Task<JobStats> GetJobStatistics([Service] IDbContextFactory<ConneXMetricsProviderContext> dbContextFactory,
        Guid jobId)
    {
        await using var connexDb = await dbContextFactory.CreateDbContextAsync();
        
        var stats = await connexDb.Database.GetDbConnection().QuerySingleAsync<JobStats>(
            """
            SELECT 
                COUNT(*) AS total_jobs,
                SUM(CASE WHEN code = 0 THEN 1 ELSE 0 END) AS total_success,
                SUM(CASE WHEN code <> 0 THEN 1 ELSE 0 END) AS total_failures
            FROM programming_statistics
            WHERE related_job_id = @RelatedJobId;
            """,
            new { RelatedJobId = jobId }
        );
        
        return stats;
    }
    
    public async Task<IEnumerable<ProgrammingResultCount>> GetJobProgrammingResults([Service] IDbContextFactory<ConneXMetricsProviderContext> dbContextFactory,
        Guid jobId,
        string interval)
    {
        await using var connexDb = await dbContextFactory.CreateDbContextAsync();
        
        var programmingResultCounts = await connexDb.Database.GetDbConnection().QueryAsync<ProgrammingResultCount>(
            """
            SELECT code, code_name, COUNT(*) AS occurrences
            FROM programming_statistics
            WHERE timestamp > NOW() - @Interval::interval
                AND related_job_id = @RelatedJobId
            GROUP BY code, code_name
            ORDER BY occurrences DESC;
            """,
            new
            {
                RelatedJobId = jobId,
                Interval = interval
            }
        );
        
        return programmingResultCounts;
    }
    
    public async Task<IEnumerable<ProgrammingTimes>> GetJobProgrammingTimes([Service] IDbContextFactory<ConneXMetricsProviderContext> dbContextFactory,
        Guid jobId,
        string interval)
    {
        await using var connexDb = await dbContextFactory.CreateDbContextAsync();
        
        var programmingTimes = (await connexDb.Database.GetDbConnection().QueryAsync<ProgrammingTimes>(
            """
            SELECT 
                time_bucket('1m', timestamp) AS bucket,
                AVG(program_duration) AS avg_program_duration,
                AVG(verify_duration) AS avg_verify_duration,
                AVG(blank_check_duration) AS avg_blank_check_duration,
                AVG(erase_duration) AS avg_erase_duration,
                AVG(overhead) AS avg_overhead
            FROM programming_statistics
            WHERE related_job_id = @RelatedJobId
                AND timestamp > NOW() - @Interval::interval
            GROUP BY bucket
            ORDER BY bucket DESC;
            """,
            new
            {
                RelatedJobId = jobId,
                Interval = interval
            }
        )).ToList();

        return programmingTimes;
    }

    public async Task<IEnumerable<JobEvent>> GetJobEvents(
        [Service] IDbContextFactory<ConneXMetricsProviderContext> dbContextFactory,
        Guid jobId,
        string interval)
    {
        await using var context = await dbContextFactory.CreateDbContextAsync();
        var connection = context.Database.GetDbConnection();
        await connection.OpenAsync();

        string sql = """
                     SELECT *
                     FROM connex_job_events
                     WHERE job_identifier = @JobId
                        AND timestamp > NOW() - @Interval::interval
                     ORDER BY timestamp DESC;
                     """;

        var parameters = new { JobId = jobId.ToString(), Interval = interval };

        return await connection.QueryAsync<JobEvent>(sql, parameters);
    }
}

// public class Query
// {
//     [UsePaging(IncludeTotalCount = true)]
//     [UseFiltering]
//     [UseSorting]
//     public IQueryable<Job> GetJobs([Service] OsmoContext db) => db.Jobs;
// }
//
// public class QueryType : ObjectType<Query>
// {
//     
// }