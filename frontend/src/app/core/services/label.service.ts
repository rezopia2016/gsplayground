import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { GenerateLabelsRequest, LabelResponse, ReprintLabelRequest } from '../models/label.model';

@Injectable({ providedIn: 'root' })
export class LabelService {
  private readonly http = inject(HttpClient);
  private readonly baseUrl = `${environment.apiBaseUrl}/labels`;

  generate(request: GenerateLabelsRequest): Observable<LabelResponse[]> {
    return this.http.post<LabelResponse[]>(`${this.baseUrl}/generate`, request);
  }

  reprint(labelId: string, request: ReprintLabelRequest): Observable<LabelResponse> {
    return this.http.post<LabelResponse>(`${this.baseUrl}/${labelId}/reprint`, request);
  }
}
