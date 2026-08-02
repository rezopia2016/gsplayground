import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { CreateKitRequest, KitResponse, RecordCustodyEventRequest } from '../models/kit.model';

@Injectable({ providedIn: 'root' })
export class KitService {
  private readonly http = inject(HttpClient);
  private readonly baseUrl = `${environment.apiBaseUrl}/kits`;

  get(id: string): Observable<KitResponse> {
    return this.http.get<KitResponse>(`${this.baseUrl}/${id}`);
  }

  create(request: CreateKitRequest): Observable<KitResponse> {
    return this.http.post<KitResponse>(this.baseUrl, request);
  }

  fulfillComponent(kitId: string, componentCode: string): Observable<KitResponse> {
    return this.http.post<KitResponse>(`${this.baseUrl}/${kitId}/components/${componentCode}/fulfill`, {});
  }

  dispatch(kitId: string, actorUserId?: string, location?: string): Observable<KitResponse> {
    return this.http.post<KitResponse>(`${this.baseUrl}/${kitId}/dispatch`, { actorUserId, location });
  }

  recordCustodyEvent(kitId: string, request: RecordCustodyEventRequest): Observable<void> {
    return this.http.post<void>(`${this.baseUrl}/${kitId}/custody-events`, request);
  }
}
