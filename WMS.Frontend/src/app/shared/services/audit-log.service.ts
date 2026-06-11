import { environment } from '../../../environments/environment';
import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { ApiResponse } from '../models/api-response.model';

export interface AuditLog {
  auditId: number;
  entityName: string;
  recordId: number;
  action: string;
  createdBy: number;
  createdOn: string;
}

@Injectable({
  providedIn: 'root'
})
export class AuditLogService {
  private apiUrl = environment.apiUrl + '/auditlog';

  constructor(private http: HttpClient) {}

  getAuditLogs(startDate?: Date | null, endDate?: Date | null): Observable<ApiResponse<AuditLog[]>> {
    let params = new HttpParams();
    if (startDate) {
      const startStr = `${startDate.getFullYear()}-${(startDate.getMonth() + 1).toString().padStart(2, '0')}-${startDate.getDate().toString().padStart(2, '0')}`;
      params = params.set('startDate', startStr);
    }
    if (endDate) {
      const endStr = `${endDate.getFullYear()}-${(endDate.getMonth() + 1).toString().padStart(2, '0')}-${endDate.getDate().toString().padStart(2, '0')}`;
      params = params.set('endDate', endStr);
    }

    return this.http.get<ApiResponse<AuditLog[]>>(this.apiUrl, { params });
  }
}
