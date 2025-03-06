using System.ComponentModel.DataAnnotations.Schema;

namespace Osmo.ConneX.Models;

/// <summary>
/// Metadata for ConneX services.
/// </summary>
[Table("connex_metadata")]
public class ConneXMetaData
{
    /// <summary>
    /// Gets or sets the identifier.
    /// </summary>
    [Column("id")]
    public int Id { get; set; }
    
    /// <summary>
    /// Gets or sets the key.
    /// </summary>
    [Column("key")]
    public string Key { get; set; }
    
    /// <summary>
    /// Gets or sets the value.
    /// </summary>
    [Column("value")]
    public string Value { get; set; }
}