using Osmo.Common.Database.Attributes;
using Osmo.Common.Database.Models;
using System.ComponentModel.DataAnnotations.Schema;

namespace Osmo.ConneX.Models;

[Table(name: "programming_statistics")]
public class ProgrammingStatistic : BaseMetric
{
    [IndexWithHyperTableColumn]
    [Column(name: "related_device_id")]
    public Guid RelatedDeviceId { get; set; }
    
    [Column(name: "related_job_id")]
    public Guid RelatedJobId { get; set; }
    
    [Column(name: "related_programmer_id")]
    public Guid RelatedProgrammerId { get; set; }
    
    [Column(name: "code")]
    public int Code { get; set; }
    
    [Column(name: "code_name")]
    public string CodeName { get; set; }
    
    [Column(name: "program_duration")]
    public int ProgramDuration { get; set; }
    
    [Column(name: "verify_duration")]
    public int VerifyDuration { get; set; }
    
    [Column(name: "blank_check_duration")]
    public int BlankCheckDuration { get; set; }
    
    [Column(name: "erase_duration")]
    public int EraseDuration { get; set; }
    
    [Column(name: "bytes_programmed")]
    public long BytesProgrammed { get; set; }
    
    [Column(name: "overhead")]
    public int Overhead { get; set; }
    
    [Column(name: "overall")]
    public int Overall { get; set; }
}