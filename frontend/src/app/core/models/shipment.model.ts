export type ShipmentStatus = 'Created' | 'Ready' | 'Dispatched' | 'InTransit' | 'Delivered' | 'Exception' | 'Cancelled';
export type ShipmentDirection = 'Outbound' | 'Inbound' | 'Return';
export type MaterialClassification = 'Standard' | 'BiologicalSample' | 'DryIce' | 'HazardousMaterial' | 'TemperatureControlled';

export interface ShipmentResponse {
  id: string;
  shipmentNumber: string;
  status: ShipmentStatus;
  direction: ShipmentDirection;
  originCountryCode: string;
  destinationCountryCode: string;
  carrierCode?: string;
  carrierServiceCode?: string;
  carrierTrackingNumber?: string;
  freightCost?: number;
  currencyCode: string;
  createdAtUtc: string;
  dispatchedAtUtc?: string;
  deliveredAtUtc?: string;
}

export interface CreateShipmentRequest {
  d365OrderReference?: string;
  direction: ShipmentDirection;
  originCountryCode: string;
  destinationCountryCode: string;
  originAddress: string;
  destinationAddress: string;
  materialClassification: MaterialClassification;
  requiresTemperatureControl: boolean;
  customerType?: string;
  isVipHandling: boolean;
  kitId?: string;
}

export interface RateQuoteRequest {
  originCountryCode: string;
  destinationCountryCode: string;
  totalWeightKg: number;
  materialClassification: MaterialClassification;
  requiresTemperatureControl: boolean;
}

export interface RateQuoteResponse {
  carrierCode: string;
  serviceCode: string;
  serviceName: string;
  amount: number;
  currencyCode: string;
  estimatedTransitDays: number;
}
