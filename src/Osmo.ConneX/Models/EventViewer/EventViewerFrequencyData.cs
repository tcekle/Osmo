namespace Osmo.ConneX.Models.EventViewer;

internal record EventViewerFrequencyData
{
    public DateTime Time { get; set; }
    public long Value { get; set; }
}