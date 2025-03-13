using Osmo.Common.Database.Attributes;
using Osmo.Common.Database.Models;
using System.ComponentModel.DataAnnotations.Schema;

namespace Osmo.ConneX.Models;

[Table(name: "handler_statistics")]
public class HandlerStatistics : BaseMetric
{
    [Column(name: "related_message_id")]
    public Guid RelatedMessageId { get; set; }
    
    [IndexWithHyperTableColumn]
    [Column(name: "related_handler_id")]
    public string RelatedHandlerId { get; set; }
    
    [IndexWithHyperTableColumn]
    [Column(name: "session_id")]
    public string SessionId { get; set; }
    
    [Column(name: "total_pass")]
    public int TotalPass { get; set; }
    
    [Column(name: "total_fail")]
    public int TotalFail { get; set; }
    
    [Column(name: "uph")]
    public int Uph { get; set; }
    
    [Column(name: "system_yield")]
    public double SystemYield { get; set; }
    
    [Column(name: "handler_yield")]
    public double HandlerYield { get; set; }
    
    [Column(name: "programmer_yield")]
    public double ProgrammerYield { get; set; }
    
    [Column(name: "devices_failed_on_programmer")]
    public int DevicesFailedOnProgrammer { get; set; }
    
    [Column(name: "devices_picked_input")]
    public int DevicesPickedInput { get; set; }
    
    [Column(name: "devices_failed_on_laser")]
    public int DevicesFailedOnLaser { get; set; }
    
    [Column(name: "devices_failed_on_3d_system")]
    public int DevicesFailedOn3DSystem { get; set; }
    
    [Column(name: "devices_failed_vision")]
    public int DevicesFailedVision { get; set; }
    
    [Column(name: "devices_failed_rest")]
    public int DevicesFailedREST { get; set; }
    
    [Column(name: "job_processing_time")]
    public int JobProcessingTime { get; set; }
    
    [Column(name: "job_assistance_time")]
    public int JobAssistanceTime { get; set; }
    
    [Column(name: "job_completion_estimate")]
    public DateTime JobCompletionEstimate { get; set; }
}