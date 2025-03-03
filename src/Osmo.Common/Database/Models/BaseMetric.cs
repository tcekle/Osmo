using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Osmo.Common.Database.Models;

using Attributes;

/// <summary>
/// Abstract class representing a time series metric 
/// </summary>
public abstract class BaseMetric
{
    /// <summary>
    /// Gets or sets the database ID
    /// </summary>
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Column(name: "id")]
    public long Id { get; set; }
        
    /// <summary>
    /// Gets or sets the metric's timestamp
    /// </summary>
    [HyperTableColumn]
    [Column(name:"timestamp", TypeName = "timestamp with time zone")]
    public DateTime TimeStamp { get; set; }

    /// <summary>
    /// Creates a new instance of <see cref="BaseMetric"/>
    /// </summary>
    protected BaseMetric()
    {
        TimeStamp = DateTime.UtcNow;
    }
}