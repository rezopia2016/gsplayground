import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { CarrierModel } from '../models/carrier.model';

@Injectable({ providedIn: 'root' })
export class CarrierService {
  private readonly http = inject(HttpClient);
  private readonly baseUrl = `${environment.apiBaseUrl}/carriers`;

  list(): Observable<CarrierModel[]> {
    return this.http.get<CarrierModel[]>(this.baseUrl);
  }
}
