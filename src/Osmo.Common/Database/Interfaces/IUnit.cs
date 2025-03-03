namespace Osmo.Common.Database.Interfaces;

/// <summary>
/// Interface for a unit.
/// </summary>
public interface IUnit
{
    /// <summary>
    /// Gets or sets the unique identifier of the unit.
    /// </summary>
    Guid Id { get; set; }
    
    /// <summary>
    /// Gets or sets the name of the unit.
    /// </summary>
    string Name { get; set; }
    
    /// <summary>
    /// Gets or sets the creation date of the unit.
    /// </summary>
    DateTime CreatedAt { get; set; }
}