namespace Osmo.Common.Database.Models;

/// <summary>
/// An algorithm that can be run on a device.
/// </summary>
public class Algorithm : Unit
{
    /// <summary>
    /// Gets or sets the related device identifier.
    /// </summary>
    public Guid RelatedDeviceId { get; set; }
    
    /// <summary>
    /// Gets or sets the algorithm identifier.
    /// </summary>
    public string AlgorithmId { get; set; }
    
    /// <summary>
    /// Gets or sets the algorithm version.
    /// </summary>
    public string AlgorithmVersion { get; set; }
}