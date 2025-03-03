using System.ComponentModel.DataAnnotations.Schema;

namespace Osmo.Common.Database.Models;

public class ProgrammableDevice : Unit
{
    [Column(name: "device_name")]
    public string DeviceName 
    { 
        get => Name;
        set => Name = value; 
    }
    
    [Column(name: "device_manufacturer")]
    public string DeviceManufacturer { get; set; }
    
    [Column(name: "device_type")]
    public string DeviceType { get; set; }
}