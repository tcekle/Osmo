using Dapper;
using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using Microsoft.JSInterop;
using Osmo.Common.Database.Models;
using Osmo.ConneX.Providers;
using Osmo.Database;

namespace Osmo.ConneX.Ui;

public partial class DeviceDetails
{
    private ProgrammableDevice _device;
    private IJSObjectReference _jsModule;

    [Inject] private IDbContextFactory<OsmoContext> OsmoContextFactory { get; set; }
    [Inject] private IDbContextFactory<ConneXMetricsProviderContext> ConnexMetricsProviderContextFactory { get; set; }
    [Inject] private IJSRuntime JsRuntime { get; set; }
    
    [Parameter]
    public string DeviceId { get; set; }

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
        
        _device = await osmoDb.ProgrammableDevices.FirstOrDefaultAsync(d => d.Id == Guid.Parse(DeviceId));
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
        if (!firstRender)
        {
            return;
        }
        
        _jsModule = await JsRuntime.InvokeAsync<IJSObjectReference>("import", "./_content/Osmo.ConneX/script/deviceDetails.js");
        await CalculateStatistics();
    }

    private async Task CalculateStatistics()
    {
        await using var connexDb = await ConnexMetricsProviderContextFactory.CreateDbContextAsync();

        var result = await connexDb.Database.GetDbConnection().QueryAsync<JobCount>(
            """
            SELECT time_bucket('1 day', timestamp) AS day, COUNT(*) AS total_jobs
            FROM programming_statistics
            WHERE related_device_id = @RelatedDeviceId
            GROUP BY day
            ORDER BY day DESC
            """,
            new { RelatedDeviceId = _device.Id }
        );
        
        await _jsModule.InvokeVoidAsync("renderJobCount", "job-count", new object());
        // connexDb.Database.GetDbConnection().QueryAsync<
    }

    private record JobCount(DateTime day, long total_jobs);
}