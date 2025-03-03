namespace Osmo.ConneX;

public class RootObject
{
    public AuditData AuditData { get; set; }
}

public class AuditData
{
    public string _SchemaVersion { get; set; }
    public string _AuditRecordId { get; set; }
    public Records Records { get; set; }
}

public class Records
{
    public AuditRecord[] AuditRecord { get; set; }
}

public class AuditRecord
{
    public string TimeStamp { get; set; }
    public Programmer Programmer { get; set; }
    public Job Job { get; set; }
    public PartDetail PartDetail { get; set; }
    public object SerialData { get; set; }
    public HandlerInfo HandlerInfo { get; set; }
}

public class Programmer
{
    public string Class { get; set; }
    public string FirmwareVersion { get; set; }
    public string SerialNumber { get; set; }
    public string SystemVersion { get; set; }
    public string ProgrammerIP { get; set; }
    public Adapter Adapter { get; set; }
}

public class Adapter
{
    public string AdapterId { get; set; }
    public string AdapterSerialNumber { get; set; }
    public string CleanCount { get; set; }
    public string LifetimeActuationCount { get; set; }
    public string LifetimeContinuityFailCount { get; set; }
    public string LifetimeFailCount { get; set; }
    public string LifetimePassCount { get; set; }
    public string SocketIndex { get; set; }
    public string AdapterState { get; set; }
}

public class Job
{
    public string AlgorithmId { get; set; }
    public string DeviceID { get; set; }
    public string AlgoVersion { get; set; }
    public string JobId { get; set; }
    public string JobName { get; set; }
    public string JobDescription { get; set; }
    public string DeviceName { get; set; }
    public string DeviceManufacturer { get; set; }
    public string DeviceType { get; set; }
}

public class PartDetail
{
    public string ChipId { get; set; }
    public string RawChipId { get; set; }
    public Result Result { get; set; }
}

public class Result
{
    public string SocketIndex { get; set; }
    public string Code { get; set; }
    public string CodeName { get; set; }
    public string BytesProgrammed { get; set; }
    public string ProgramDuration { get; set; }
    public string VerifyDuration { get; set; }
    public string BlankCheckDuration { get; set; }
    public string EraseDuration { get; set; }
    public string Overhead { get; set; }
    public AlgoDeviceDetails AlgoDeviceDetails { get; set; }
    public string ErrorMessage { get; set; }
}

public class AlgoDeviceDetails
{
    public string Version { get; set; }
    public Device Device { get; set; }
    public Sentrix Sentrix { get; set; }
    public string ReadAndLog { get; set; }
}

public class Device
{
    public string Se050Variant { get; set; }
    public string Se050OefId { get; set; }
    public string UserIdForReservedIdFactoryReset { get; set; }
    public string LockState { get; set; }
    public string ScpPlatform { get; set; }
    public string AppletLevelScp03 { get; set; }
    public string KeyPairNum { get; set; }
    public string KeyPair1Source { get; set; }
    public string KeyPair1Type { get; set; }
    public string KeyPair1ObjectID { get; set; }
    public string KeyPair1PolicyChoice { get; set; }
    public string KeyPair1Public { get; set; }
    public string Certificate1ObjectID { get; set; }
    public string Certificate1PolicyChoice { get; set; }
    public string Certificate1SignatureType { get; set; }
    public string Certificate1 { get; set; }
    public string KeyPair2Source { get; set; }
    public string KeyPair2Type { get; set; }
    public string KeyPair2ObjectID { get; set; }
    public string KeyPair2PolicyChoice { get; set; }
    public string KeyPair2Public { get; set; }
    public string KeyPair3Source { get; set; }
    public string KeyPair3Type { get; set; }
    public string KeyPair3ObjectID { get; set; }
    public string KeyPair3PolicyChoice { get; set; }
    public string KeyPair3Public { get; set; }
    public string Certificate3ObjectID { get; set; }
    public string Certificate3PolicyChoice { get; set; }
    public string Certificate3SignatureType { get; set; }
    public string Certificate3 { get; set; }
    public string KeyPair4Source { get; set; }
    public string KeyPair4Type { get; set; }
    public string KeyPair4ObjectID { get; set; }
    public string KeyPair4PolicyChoice { get; set; }
    public string KeyPair4Public { get; set; }
    public string KeyPair5Source { get; set; }
    public string KeyPair5Type { get; set; }
    public string KeyPair5ObjectID { get; set; }
    public string KeyPair5PolicyChoice { get; set; }
    public string KeyPair5Public { get; set; }
    public string KeyPair6Source { get; set; }
    public string KeyPair6Type { get; set; }
    public string KeyPair6ObjectID { get; set; }
    public string KeyPair6PolicyChoice { get; set; }
    public string KeyPair6Public { get; set; }
    public string SymmetricKeyNumber { get; set; }
    public string PublicKeyNum { get; set; }
    public string PublicKey1 { get; set; }
    public string PublicKey1ObjectID { get; set; }
    public string PublicKey1PolicyChoice { get; set; }
    public string PublicKey2 { get; set; }
    public string PublicKey2ObjectID { get; set; }
    public string PublicKey2PolicyChoice { get; set; }
}

public class Sentrix
{
    public string AttestationEccCertificateValidated { get; set; }
    public string AttestationRsaCertificateValidated { get; set; }
}

public class HandlerInfo
{
    public string Name { get; set; }
    public string IpAddresses { get; set; }
    public Version Version { get; set; }
    public string MachineSNID { get; set; }
    public string MachineName { get; set; }
    public string FactoryName { get; set; }
    public string PCSerialNumber { get; set; }
    public int LicenseLevel { get; set; }
}

public class Version
{
    public int Major { get; set; }
    public int Minor { get; set; }
    public int Build { get; set; }
    public int Revision { get; set; }
    public int MajorRevision { get; set; }
    public int MinorRevision { get; set; }
}

