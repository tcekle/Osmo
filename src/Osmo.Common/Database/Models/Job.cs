namespace Osmo.Common.Database.Models;

public class Job : Unit
{
    public Guid RelatedAlgorithmId { get; set; }
    
    public Guid GivenJobId { get; set; }

    public string JobName
    {
        get => Name; 
        set => Name = value;
    }
    
    public string JobDescription { get; set; }
    public string JobChecksum { get; set; }
    public string SettingChecksum { get; set; }
}

/*
 *     "Job": {
      "AlgorithmId": "31983706628437248",
      "JobId": "69467bae-9fa3-460f-aa6d-9ef233ddd62b",
      "JobName": "WD DO4-128G FS testing",
      "JobDescription": "",
      "DeviceName": "SDINFDO4-128G",
      "DeviceManufacturer": "Western Digital",
      "DeviceType": "Ufs",
      "DeviceID": "29089",
      "AlgoVersion": "3.3.53",
      "JobChecksum": "D50802DB",
      "SettingChecksum": "2C1C9B00"
    },
 */