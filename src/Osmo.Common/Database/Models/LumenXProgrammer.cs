using System.ComponentModel.DataAnnotations.Schema;

namespace Osmo.Common.Database.Models;

/// <summary>
/// A LumenX programmer.
/// </summary>
public class LumenXProgrammer : Producer
{
    /// <summary>
    /// Gets or sets the firmware version.
    /// </summary>
    [Column(name: "firmware_version")]
    public string FirmwareVersion { get; set; }
    
    /// <summary>
    /// Gets or sets the system version.
    /// </summary>
    [Column(name: "serial_number")]
    public string SystemVersion { get; set; }
    
    /// <summary>
    /// Creates a new instance of the <see cref="LumenXProgrammer"/> class.
    /// </summary>
    public LumenXProgrammer()
    {
        Type = "LumenX";
    }
}