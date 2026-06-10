import { environment } from '../../../environments/environment';
import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { ApiResponse, PagedData } from '../models/api-response.model';
import { AttendanceRecord, CheckInRequest, AttendanceFilter } from '../models/attendance.model';

@Injectable({
  providedIn: 'root'
})
export class AttendanceService {
  private apiUrl = environment.apiUrl + '/attendance';

  constructor(private http: HttpClient) {}

  checkIn(request: CheckInRequest): Observable<ApiResponse<AttendanceRecord>> {
    return this.http.post<ApiResponse<AttendanceRecord>>(`${this.apiUrl}/check-in`, request);
  }

  checkOut(): Observable<ApiResponse<AttendanceRecord>> {
    return this.http.post<ApiResponse<AttendanceRecord>>(`${this.apiUrl}/check-out`, {});
  }

  getTodayStatus(): Observable<ApiResponse<AttendanceRecord | null>> {
    return this.http.get<ApiResponse<AttendanceRecord | null>>(`${this.apiUrl}/today`);
  }

  getMyHistory(filter: AttendanceFilter): Observable<ApiResponse<PagedData<AttendanceRecord>>> {
    let params = new HttpParams()
      .set('pageNumber', filter.pageNumber.toString())
      .set('pageSize', filter.pageSize.toString());
      
    if (filter.fromDate) params = params.set('fromDate', filter.fromDate);
    if (filter.toDate) params = params.set('toDate', filter.toDate);
    if (filter.searchTerm) params = params.set('searchTerm', filter.searchTerm);

    return this.http.get<ApiResponse<PagedData<AttendanceRecord>>>(`${this.apiUrl}/my-history`, { params });
  }

  getAllAttendance(filter: AttendanceFilter): Observable<ApiResponse<PagedData<AttendanceRecord>>> {
    let params = new HttpParams()
      .set('pageNumber', filter.pageNumber.toString())
      .set('pageSize', filter.pageSize.toString());
      
    if (filter.fromDate) params = params.set('fromDate', filter.fromDate);
    if (filter.toDate) params = params.set('toDate', filter.toDate);
    if (filter.searchTerm) params = params.set('searchTerm', filter.searchTerm);

    return this.http.get<ApiResponse<PagedData<AttendanceRecord>>>(this.apiUrl, { params });
  }
}
