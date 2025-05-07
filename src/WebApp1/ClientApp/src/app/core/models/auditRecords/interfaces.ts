export interface AuditData {
  "@SchemaVersion": string;
  "@AuditRecordId": string;
  Records: Records;
}

export interface Records {
  AuditRecord: AuditRecord[];
}

export interface AuditRecord {
  TimeStamp: string;
  Programmer: Programmer;
  Job: Job;
  PartDetail: PartDetail;
  SerialData: any;
  HandlerInfo: HandlerInfo;
}

export interface Programmer {
  Class: string;
  FirmwareVersion: string;
  SerialNumber: string;
  SystemVersion: string;
  ProgrammerIP: string;
  Adapter: Adapter;
}

export interface Adapter {
  AdapterId: string;
  AdapterSerialNumber: string;
  CleanCount: string;
  LifetimeActuationCount: string;
  LifetimeContinuityFailCount: string;
  LifetimeFailCount: string;
  LifetimePassCount: string;
  SocketIndex: string;
  AdapterState: string;
}

export interface Job {
  AlgorithmId: string;
  DeviceID: string;
  AlgoVersion: string;
  JobId: string;
  JobName: string;
  JobDescription: string;
  DeviceName: string;
  DeviceManufacturer: string;
  DeviceType: string;
}

export interface PartDetail {
  ChipId: string;
  RawChipId: string;
  Result: Result;
}

export interface Result {
  SocketIndex: string;
  Code: string;
  CodeName: string;
  BytesProgrammed: string;
  ProgramDuration: string;
  VerifyDuration: string;
  BlankCheckDuration: string;
  EraseDuration: string;
  Overhead: string;
  AlgoDeviceDetails: AlgoDeviceDetails;
  ErrorMessage: string;
}

export interface AlgoDeviceDetails {
  Version: string;
  Device: Record<string, string>;
  Sentrix: {
    AttestationEccCertificateValidated: string;
    AttestationRsaCertificateValidated: string;
  };
  ReadAndLog: Record<string, unknown>;
}

export interface HandlerInfo {
  Name: string;
  IpAddresses: string;
  Version: Version;
  MachineSNID: string;
  MachineName: string;
  FactoryName: string;
  PCSerialNumber: string;
  LicenseLevel: number;
}

export interface Version {
  Major: number;
  Minor: number;
  Build: number;
  Revision: number;
  MajorRevision: number;
  MinorRevision: number;
}
