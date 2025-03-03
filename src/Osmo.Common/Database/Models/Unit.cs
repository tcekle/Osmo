using Osmo.Common.Database.Interfaces;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Osmo.Common.Database.Models;

/// <summary>
/// A default implementation of the IUnit interface.
/// </summary>
public abstract class Unit : IUnit
{
    /// <summary>
    /// Gets or sets the unique identifier of the unit.
    /// </summary>
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Column(name: "id")]
    public Guid Id { get; set; } = Guid.CreateVersion7();
    
    /// <summary>
    /// Gets or sets the name of the unit.
    /// </summary>
    [Column(name: "name")]
    public string Name { get; set; }
    
    /// <summary>
    /// Gets or sets the creation date of the unit.
    /// </summary>
    [Column(name: "created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}