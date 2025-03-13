using System.ComponentModel.DataAnnotations.Schema;

namespace Osmo.ConneX.Models;


internal abstract class ConneXEvent
{
    [Column("id")]
    public int Id { get; set; }
    
    [Column("type")]
    public string Type { get; set; }
    
    [Column("title")]
    public string Title { get; set; }
    
    [Column("message")]
    public string Message { get; set; }
    
    [Column("rawdata", TypeName = "jsonb")]
    public string RawData { get; set; }
    
    [Column("timestamp", TypeName = "timestamp with time zone")]
    public DateTime Timestamp { get; set; }
    
    [Column("related_message_id")]
    public Guid RelatedMessageId { get; set; }
}
