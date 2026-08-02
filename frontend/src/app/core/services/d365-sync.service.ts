import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { D365SyncRecordModel } from '../models/carrier.model';

/** Backs the reconciliation dashboard (UR-051 / E7.S6) — surfaces D365 outbox health. */
@Injectable({ providedIn: 'root' })
export class D365SyncService {
  private readonly http = inject(HttpClient);
  private readonly baseUrl = `${environment.apiBaseUrl}/d365-sync`;

  getPending(maxAttempts = 5): Observable<D365SyncRecordModel[]> {
    return this.http.get<D365SyncRecordModel[]>(`${this.baseUrl}/pending`, { params: { maxAttempts } });
  }
}
