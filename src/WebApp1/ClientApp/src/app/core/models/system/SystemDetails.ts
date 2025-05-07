export interface SystemDetails {
  ipAddress: string;
  hostName: string;
  machineFactory: string;
  handlerType: string;
  entity: SystemEntity;
}

export interface SystemEntity {
  entityName: string;
}
