using Microsoft.JSInterop;
using Osmo.ConneX.Ui.Components;

namespace Osmo.ConneX.Services;

public class TimeSeriesJavascriptInterop
{
    private readonly IJSRuntime _jsRuntime;
    private IJSObjectReference _jsModule;

    public TimeSeriesJavascriptInterop(IJSRuntime jsRuntime)
    {
        _jsRuntime = jsRuntime;
        
    }
    
    public async ValueTask RenderTimeSeries(string identifier, TimeSeries.TimeSeriesTrace trace, TimeSeries.TimeSeriesLayout layout)
    {
        await Initialize();
        await _jsModule.InvokeVoidAsync("renderTimeSeries", identifier, trace, layout);
    }

    private async ValueTask Initialize()
    {
        if (_jsModule != null)
        {
            return;
        }
        
        _jsModule = await _jsRuntime.InvokeAsync<IJSObjectReference>("import", "./_content/Osmo.ConneX/script/timeSeries.js");
    }
}