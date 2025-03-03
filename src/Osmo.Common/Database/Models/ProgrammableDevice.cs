using System.ComponentModel.DataAnnotations.Schema;

namespace Osmo.Common.Database.Models;

/// <summary>
/// A programmable device.
/// </summary>
public class ProgrammableDevice : Unit
{
    /// <summary>
    /// Gets or sets the name of the device.
    /// </summary>
    [Column(name: "device_name")]
    public string DeviceName 
    { 
        get => Name;
        set => Name = value; 
    }
    
    /// <summary>
    /// Gets or sets the manufacturer of the device.
    /// </summary>
    [Column(name: "device_manufacturer")]
    public string DeviceManufacturer { get; set; }
    
    /// <summary>
    /// Gets or sets the type of the device.
    /// </summary>
    [Column(name: "device_type")]
    public string DeviceType { get; set; }
}