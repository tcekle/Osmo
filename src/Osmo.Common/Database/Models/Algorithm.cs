namespace Osmo.Common.Database.Models;

public class Algorithm : Unit
{
    public Guid RelatedDeviceId { get; set; }
    
    public string AlgorithmId { get; set; }
    
    public string AlgorithmVersion { get; set; }
}