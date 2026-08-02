export interface CarrierServiceModel {
  serviceCode: string;
  serviceName: string;
  supportsTemperatureControl: boolean;
  supportsBiologicalMaterial: boolean;
}

export interface CarrierModel {
  id: string;
  code: string;
  name: string;
  adapterKey: string;
  isActive: boolean;
  supportsEdi: boolean;
  priority: number;
  services: CarrierServiceModel[];
}

export interface D365SyncRecordModel {
  id: string;
  direction: 'Inbound' | 'Outbound';
  entityType: 'Order' | 'CustomerAddress' | 'BillingEvent' | 'InventoryEvent' | 'ComplianceDocument';
  status: 'Pending' | 'Synced' | 'Failed' | 'Retrying';
  attemptCount: number;
  lastError?: string;
  createdAtUtc: string;
  lastAttemptAtUtc?: string;
}
