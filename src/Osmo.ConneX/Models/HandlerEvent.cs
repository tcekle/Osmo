using System.ComponentModel.DataAnnotations.Schema;

namespace Osmo.ConneX.Models;

[Table("connex_handler_events")]
internal class HandlerEvent : ConneXEvent
{
    [Column(name: "handler_identifier")]
    public string HandlerIdentifier { get; set; }
    
    [Column(name: "handler_name")]
    public string HandlerName { get; set; }
    
    [Column(name: "session_id")]
    public string SessionId { get; set; }
}