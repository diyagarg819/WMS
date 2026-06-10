import { environment } from '../../../environments/environment';
import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { ApiResponse, PagedData } from '../models/api-response.model';

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

  getAuditLogs(pageNumber: number, pageSize: number): Observable<ApiResponse<PagedData<AuditLog>>> {
    const params = new HttpParams()
      .set('pageNumber', pageNumber.toString())
      .set('pageSize', pageSize.toString());

    return this.http.get<ApiResponse<PagedData<AuditLog>>>(this.apiUrl, { params });
  }
}
