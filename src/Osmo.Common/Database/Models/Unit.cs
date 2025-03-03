using Osmo.Common.Database.Interfaces;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Osmo.Common.Database.Models;

public abstract class Unit : IUnit
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Column(name: "id")]
    public Guid Id { get; set; } = Guid.CreateVersion7();
    
    [Column(name: "name")]
    public string Name { get; set; }
    
    [Column(name: "created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}