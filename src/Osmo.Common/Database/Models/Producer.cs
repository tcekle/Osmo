using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Osmo.Common.Database.Models;

using Interfaces;

/// <summary>
/// A default implementation of the IProducer interface.
/// </summary>
public abstract class Producer : IProducer
{
    /// <summary>
    /// Gets or sets the unique identifier of the producer.
    /// </summary>
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Column(name: "id")]
    public Guid Id { get; set; } = Guid.CreateVersion7();
    
    /// <summary>
    /// Gets or sets the name of the producer.
    /// </summary>
    [Column(name: "name")]
    public string Name { get; set; }
    
    /// <summary>
    /// Gets or sets the unique identifier of the producer.
    /// </summary>
    [Column(name: "unique_identifier")]
    public string UniqueIdentifier { get; set; }
    
    /// <summary>
    /// Gets or sets the type of the producer.
    /// </summary>
    [Column(name: "type")]
    public string Type { get; set; }
}