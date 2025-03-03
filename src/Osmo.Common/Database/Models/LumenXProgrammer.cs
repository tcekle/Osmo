using System.ComponentModel.DataAnnotations.Schema;

namespace Osmo.Common.Database.Models;

public class LumenXProgrammer : Producer
{
    [Column(name: "firmware_version")]
    public string FirmwareVersion { get; set; }
    
    [Column(name: "serial_number")]
    public string SystemVersion { get; set; }
    
    public LumenXProgrammer()
    {
        Type = "LumenX";
    }
}