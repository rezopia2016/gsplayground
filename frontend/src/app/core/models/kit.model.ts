export type KitStatus = 'Created' | 'Assembling' | 'ReadyForDispatch' | 'Dispatched' | 'Collected' | 'InTransit' | 'Returned' | 'Received' | 'Incomplete';
export type CustodyAction = 'Created' | 'Packed' | 'Dispatched' | 'PickedUp' | 'Transferred' | 'Received' | 'Returned';

export interface KitComponentResponse {
  componentCode: string;
  componentName: string;
  quantity: number;
  isRequired: boolean;
  isFulfilled: boolean;
}

export interface KitResponse {
  id: string;
  kitNumber: string;
  kitTypeCode: string;
  status: KitStatus;
  isComplete: boolean;
  components: KitComponentResponse[];
}

export interface KitComponentRequest {
  componentCode: string;
  componentName: string;
  quantity: number;
  isRequired: boolean;
  consumableSku?: string;
}

export interface CreateKitRequest {
  kitTypeCode: string;
  d365OrderReference?: string;
  components: KitComponentRequest[];
}

export interface RecordCustodyEventRequest {
  action: CustodyAction;
  location?: string;
  actorUserId?: string;
  notes?: string;
  shipmentId?: string;
}
