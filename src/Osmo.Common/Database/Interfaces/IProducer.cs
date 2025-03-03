namespace Osmo.Common.Database.Interfaces;

/// <summary>
/// Interface for a producer.
/// </summary>
public interface IProducer
{
    /// <summary>
    /// Gets or sets the name of the producer.
    /// </summary>
    public string Name { get; set; }
    
    /// <summary>
    /// Gets or sets the unique identifier of the producer.
    /// </summary>
    public string UniqueIdentifier { get; set; }
    
    /// <summary>
    /// Gets or sets the type of the producer.
    /// </summary>
    string Type { get; set; }
}