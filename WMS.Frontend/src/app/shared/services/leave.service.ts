import { environment } from '../../../environments/environment';
import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { ApiResponse, PagedData } from '../models/api-response.model';
import { LeaveRecord, ApplyLeaveRequest, UpdateLeaveStatusRequest, LeaveFilter } from '../models/leave.model';

@Injectable({
  providedIn: 'root'
})
export class LeaveService {
  private apiUrl = environment.apiUrl + '/leave';

  constructor(private http: HttpClient) {}

  applyLeave(request: ApplyLeaveRequest): Observable<ApiResponse<LeaveRecord>> {
    return this.http.post<ApiResponse<LeaveRecord>>(`${this.apiUrl}/apply`, request);
  }

  cancelLeave(leaveId: number): Observable<ApiResponse<any>> {
    return this.http.post<ApiResponse<any>>(`${this.apiUrl}/cancel/${leaveId}`, {});
  }

  getMyHistory(filter: LeaveFilter): Observable<ApiResponse<PagedData<LeaveRecord>>> {
    let params = this.buildParams(filter);
    return this.http.get<ApiResponse<PagedData<LeaveRecord>>>(`${this.apiUrl}/my-history`, { params });
  }

  getTeamLeaves(filter: LeaveFilter): Observable<ApiResponse<PagedData<LeaveRecord>>> {
    let params = this.buildParams(filter);
    return this.http.get<ApiResponse<PagedData<LeaveRecord>>>(`${this.apiUrl}/team`, { params });
  }

  getAllLeaves(filter: LeaveFilter): Observable<ApiResponse<PagedData<LeaveRecord>>> {
    let params = this.buildParams(filter);
    return this.http.get<ApiResponse<PagedData<LeaveRecord>>>(`${this.apiUrl}/all`, { params });
  }

  approveOrReject(leaveId: number, request: UpdateLeaveStatusRequest): Observable<ApiResponse<LeaveRecord>> {
    return this.http.post<ApiResponse<LeaveRecord>>(`${this.apiUrl}/approve-reject/${leaveId}`, request);
  }

  private buildParams(filter: LeaveFilter): HttpParams {
    let params = new HttpParams()
      .set('pageNumber', filter.pageNumber.toString())
      .set('pageSize', filter.pageSize.toString());
      
    if (filter.searchTerm) params = params.set('searchTerm', filter.searchTerm);
    if (filter.status) params = params.set('status', filter.status);

    return params;
  }
}
