import { environment } from '../../../environments/environment';
import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { ApiResponse } from '../models/api-response.model';
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

  private formatDate(date: any): string | null {
    if (!date) return null;
    const d = new Date(date);
    const year = d.getFullYear();
    const month = ('0' + (d.getMonth() + 1)).slice(-2);
    const day = ('0' + d.getDate()).slice(-2);
    return `${year}-${month}-${day}`;
  }

  getMyHistory(filter: AttendanceFilter): Observable<ApiResponse<AttendanceRecord[]>> {
    let params = new HttpParams();
    const fromStr = this.formatDate(filter.fromDate);
    const toStr = this.formatDate(filter.toDate);
    
    if (fromStr) params = params.set('fromDate', fromStr);
    if (toStr) params = params.set('toDate', toStr);
    if (filter.searchTerm) params = params.set('searchTerm', filter.searchTerm);

    return this.http.get<ApiResponse<AttendanceRecord[]>>(`${this.apiUrl}/my-history`, { params });
  }

  getAllAttendance(filter: AttendanceFilter): Observable<ApiResponse<AttendanceRecord[]>> {
    let params = new HttpParams();
    const fromStr = this.formatDate(filter.fromDate);
    const toStr = this.formatDate(filter.toDate);
    
    if (fromStr) params = params.set('fromDate', fromStr);
    if (toStr) params = params.set('toDate', toStr);
    if (filter.searchTerm) params = params.set('searchTerm', filter.searchTerm);

    return this.http.get<ApiResponse<AttendanceRecord[]>>(this.apiUrl, { params });
  }
}
