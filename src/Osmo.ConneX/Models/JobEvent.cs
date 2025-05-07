using System.ComponentModel.DataAnnotations.Schema;

namespace Osmo.ConneX.Models;

[Table("connex_job_events")]
public class JobEvent : ConneXEvent
{
    [Column(name: "job_identifier")]
    public string JobIdentifier { get; set; }
}