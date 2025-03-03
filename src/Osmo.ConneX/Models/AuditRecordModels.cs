namespace Osmo.ConneX.Models;

using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

public class ConneXAuditEntry
{
    public DateTime TimeStamp { get; set; }
    public Programmer Programmer { get; set; }
    public Job Job { get; set; }
    public PartDetail PartDetail { get; set; }
    // public HandlerInfo HandlerInfo { get; set; }
    // public object SerialData { get; set; } // Assuming it can be null
}

public class Programmer
{
    public string Class { get; set; }
    public string FirmwareVersion { get; set; }
    public string SerialNumber { get; set; }
    public string SystemVersion { get; set; }
    public string ProgrammerIP { get; set; }
    // public Adapter Adapter { get; set; }
}

public class Adapter
{
    public string AdapterId { get; set; }
    public string AdapterSerialNumber { get; set; }
    public int CleanCount { get; set; }
    public int LifetimeActuationCount { get; set; }
    public int LifetimeContinuityFailCount { get; set; }
    public int LifetimeFailCount { get; set; }
    public int LifetimePassCount { get; set; }
    public int SocketIndex { get; set; }
    public string AdapterState { get; set; }
}

public class Job
{
    public string AlgorithmId { get; set; }
    public Guid JobId { get; set; }
    public string JobName { get; set; }
    public string JobDescription { get; set; }
    public string DeviceName { get; set; }
    public string DeviceManufacturer { get; set; }
    public string DeviceType { get; set; }
    public string DeviceID { get; set; }
    public string AlgoVersion { get; set; }
    public string JobChecksum { get; set; }
    public string SettingChecksum { get; set; }
}

public class PartDetail
{
    public string ChipId { get; set; }
    public string RawChipId { get; set; }
    public Result Result { get; set; }
}

public class Result
{
    public string Code { get; set; }
    public string CodeName { get; set; }
    public int ProgramDuration { get; set; }
    public int VerifyDuration { get; set; }
    // public object Times { get; set; } // Assuming null values
    // public AlgoDeviceDetails AlgoDeviceDetails { get; set; }
    public string BlankCheckDuration { get; set; }
    public string EraseDuration { get; set; }
    public string ErrorMessage { get; set; }
    public long BytesProgrammed { get; set; }
    public string SocketIndex { get; set; }
    public string Overhead { get; set; }
    public string Overall { get; set; }
}

public class AlgoDeviceDetails
{
    public object CID { get; set; }
    public object Device { get; set; }
    public object Sentrix { get; set; }
    public string Cnt { get; set; }
}

public class HandlerInfo
{
    public string Name { get; set; }
    public string IpAddresses { get; set; }
    public string Version { get; set; }
    public string MachineSNID { get; set; }
    public string MachineName { get; set; }
    public string FactoryName { get; set; }
    public object LicenseLevel { get; set; } // Assuming null values
    public string PCSerialNumber { get; set; }
}
