import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import {
  CreateShipmentRequest,
  RateQuoteRequest,
  RateQuoteResponse,
  ShipmentResponse
} from '../models/shipment.model';

@Injectable({ providedIn: 'root' })
export class ShipmentService {
  private readonly http = inject(HttpClient);
  private readonly baseUrl = `${environment.apiBaseUrl}/shipments`;

  list(): Observable<ShipmentResponse[]> {
    return this.http.get<ShipmentResponse[]>(this.baseUrl);
  }

  get(id: string): Observable<ShipmentResponse> {
    return this.http.get<ShipmentResponse>(`${this.baseUrl}/${id}`);
  }

  create(request: CreateShipmentRequest): Observable<ShipmentResponse> {
    return this.http.post<ShipmentResponse>(this.baseUrl, request);
  }

  dispatch(id: string, carrierCode: string, serviceCode: string): Observable<ShipmentResponse> {
    return this.http.post<ShipmentResponse>(`${this.baseUrl}/${id}/dispatch`, { carrierCode, serviceCode });
  }

  cancel(id: string, reason?: string): Observable<ShipmentResponse> {
    return this.http.post<ShipmentResponse>(`${this.baseUrl}/${id}/cancel`, { reason });
  }

  getRates(request: RateQuoteRequest): Observable<RateQuoteResponse[]> {
    return this.http.post<RateQuoteResponse[]>(`${this.baseUrl}/rates`, request);
  }
}
