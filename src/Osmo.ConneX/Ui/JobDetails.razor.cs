using Dapper;
using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using Osmo.ConneX.Providers;
using Osmo.ConneX.Ui.Components;
using Osmo.Database;

namespace Osmo.ConneX.Ui;

public partial class JobDetails
{
    private Common.Database.Models.Job _job;
    private bool _loading = true;

    private JobStats _jobStats;
    private IEnumerable<ProgrammingResultCount> _programmingResultCounts;
    private List<ProgrammingTimes> _programmingTimes;

    private TimeSeries.TimeSeriesTrace _programmingTimeTrace;
    private TimeSeries.TimeSeriesTrace _verifyTimeTrace;
    private TimeSeries.TimeSeriesTrace _blankCheckTimeTrace;
    private TimeSeries.TimeSeriesTrace _eraseTimeTrace;
    private TimeSeries.TimeSeriesTrace _overheadTimeTrace;
    
    
    [Inject] private IDbContextFactory<OsmoContext> OsmoContextFactory { get; set; }
    [Inject] private IDbContextFactory<ConneXMetricsProviderContext> ConnexMetricsProviderContextFactory { get; set; }
    
    [Parameter]
    public string JobId { get; set; }

    /// <summary>
    /// Method invoked when the component is ready to start, having received its
    /// initial parameters from its parent in the render tree.
    /// Override this method if you will perform an asynchronous operation and
    /// want the component to refresh when that operation is completed.
    /// </summary>
    /// <returns>A <see cref="T:System.Threading.Tasks.Task" /> representing any asynchronous operation.</returns>
    protected override async Task OnInitializedAsync()
    {
        await using var osmoDb = await OsmoContextFactory.CreateDbContextAsync();
        
        _job = await osmoDb.Jobs.FirstOrDefaultAsync(j => j.Id == Guid.Parse(JobId));
    }

    /// <summary>
    /// Method invoked after each time the component has been rendered interactively and the UI has finished
    /// updating (for example, after elements have been added to the browser DOM). Any <see cref="T:Microsoft.AspNetCore.Components.ElementReference" />
    /// fields will be populated by the time this runs.
    /// This method is not invoked during prerendering or server-side rendering, because those processes
    /// are not attached to any live browser DOM and are already complete before the DOM is updated.
    /// Note that the component does not automatically re-render after the completion of any returned <see cref="T:System.Threading.Tasks.Task" />,
    /// because that would cause an infinite render loop.
    /// </summary>
    /// <param name="firstRender">
    /// Set to <c>true</c> if this is the first time <see cref="M:Microsoft.AspNetCore.Components.ComponentBase.OnAfterRender(System.Boolean)" /> has been invoked
    /// on this component instance; otherwise <c>false</c>.
    /// </param>
    /// <returns>A <see cref="T:System.Threading.Tasks.Task" /> representing any asynchronous operation.</returns>
    /// <remarks>
    /// The <see cref="M:Microsoft.AspNetCore.Components.ComponentBase.OnAfterRender(System.Boolean)" /> and <see cref="M:Microsoft.AspNetCore.Components.ComponentBase.OnAfterRenderAsync(System.Boolean)" /> lifecycle methods
    /// are useful for performing interop, or interacting with values received from <c>@ref</c>.
    /// Use the <paramref name="firstRender" /> parameter to ensure that initialization work is only performed
    /// once.
    /// </remarks>
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (!firstRender || _job == null)
        {
            return;
        }
        
        await using var connexDb = await ConnexMetricsProviderContextFactory.CreateDbContextAsync();
        _jobStats = await connexDb.Database.GetDbConnection().QuerySingleAsync<JobStats>(
            """
            SELECT 
                COUNT(*) AS total_jobs,
                SUM(CASE WHEN code = 0 THEN 1 ELSE 0 END) AS total_success,
                SUM(CASE WHEN code <> 0 THEN 1 ELSE 0 END) AS total_failures
            FROM programming_statistics
            WHERE related_job_id = @RelatedJobId;
            """,
            new { RelatedJobId = _job.Id }
        );
        
        _programmingResultCounts = await connexDb.Database.GetDbConnection().QueryAsync<ProgrammingResultCount>(
            """
            SELECT code, code_name, COUNT(*) AS occurrences
            FROM programming_statistics
            WHERE timestamp > NOW() - Interval '1 month'
                AND related_job_id = @RelatedJobId
            GROUP BY code, code_name
            ORDER BY occurrences DESC;
            """,
            new { RelatedJobId = _job.Id }
        );
        
        _programmingTimes = (await connexDb.Database.GetDbConnection().QueryAsync<ProgrammingTimes>(
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
            GROUP BY bucket
            ORDER BY bucket DESC;
            """,
            new { RelatedJobId = _job.Id }
        )).ToList();

        _programmingTimeTrace = new TimeSeries.TimeSeriesTrace
        {
            Name = "Average Programming Duration",
            X = _programmingTimes.Select(pt => pt.bucket).ToList(),
            Y = _programmingTimes.Select(pt => (double)pt.avg_program_duration).ToList()
        };
        
        _verifyTimeTrace = new TimeSeries.TimeSeriesTrace
        {
            Name = "Average Verify Duration",
            X = _programmingTimes.Select(pt => pt.bucket).ToList(),
            Y = _programmingTimes.Select(pt => (double)pt.avg_verify_duration).ToList()
        };
        
        _blankCheckTimeTrace = new TimeSeries.TimeSeriesTrace
        {
            Name = "Average Blank Check Duration",
            X = _programmingTimes.Select(pt => pt.bucket).ToList(),
            Y = _programmingTimes.Select(pt => (double)pt.avg_blank_check_duration).ToList()
        };
        
        _eraseTimeTrace = new TimeSeries.TimeSeriesTrace
        {
            Name = "Average Erase Duration",
            X = _programmingTimes.Select(pt => pt.bucket).ToList(),
            Y = _programmingTimes.Select(pt => (double)pt.avg_erase_duration).ToList()
        };
        
        _overheadTimeTrace = new TimeSeries.TimeSeriesTrace
        {
            Name = "Average Overhead Duration",
            X = _programmingTimes.Select(pt => pt.bucket).ToList(),
            Y = _programmingTimes.Select(pt => (double)pt.avg_overhead).ToList()
        };
        
        _loading = false;

        await InvokeAsync(StateHasChanged);
    }

    private record JobStats(long total_jobs, long total_success, long total_failures);
    private record ProgrammingResultCount(int code, string code_name, long occurrences);
    
    private record ProgrammingTimes(DateTime bucket, decimal avg_program_duration, decimal avg_verify_duration, decimal avg_blank_check_duration, decimal avg_erase_duration, decimal avg_overhead);
}