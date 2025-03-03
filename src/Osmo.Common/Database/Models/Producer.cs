using Osmo.Common.Database.Interfaces;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Osmo.Common.Database.Models;

public abstract class Producer : IProducer
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Column(name: "id")]
    public Guid Id { get; set; } = Guid.CreateVersion7();
    
    [Column(name: "name")]
    public string Name { get; set; }
    
    [Column(name: "unique_identifier")]
    public string UniqueIdentifier { get; set; }
    
    [Column(name: "type")]
    public string Type { get; set; }
}