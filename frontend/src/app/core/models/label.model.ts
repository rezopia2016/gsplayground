export type LabelType = 'Customer' | 'Internal' | 'Sample' | 'CollectionKit' | 'Hazard' | 'Return';
export type LabelFormat = 'Zpl' | 'Pdf' | 'Qr';

export interface GenerateLabelsRequest {
  packageId: string;
  requestedTypes: LabelType[];
  format: LabelFormat;
}

export interface LabelResponse {
  id: string;
  type: LabelType;
  format: LabelFormat;
  barcodeValue: string;
  renderedContentUri?: string;
  printCount: number;
}

export interface ReprintLabelRequest {
  reason: string;
  printerId?: string;
  printedByUserId?: string;
}
