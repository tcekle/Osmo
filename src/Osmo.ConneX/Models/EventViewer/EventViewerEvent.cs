namespace Osmo.ConneX.Models.EventViewer;

public record EventViewerEvent
{
    public string Type { get; set; }
    public string Title { get; set; }
    public string Message { get; set; }
    public string RawData { get; set; }
    public DateTime Timestamp { get; set; }
}