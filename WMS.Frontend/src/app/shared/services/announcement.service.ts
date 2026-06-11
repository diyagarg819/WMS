import { environment } from '../../../environments/environment';
import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { ApiResponse } from '../models/api-response.model';
import { AnnouncementRecord, CreateAnnouncementRequest, UpdateAnnouncementRequest } from '../models/announcement.model';

@Injectable({
  providedIn: 'root'
})
export class AnnouncementService {
  private apiUrl = environment.apiUrl + '/announcement';

  constructor(private http: HttpClient) {}

  getAll(searchTerm?: string, isActive?: boolean): Observable<ApiResponse<AnnouncementRecord[]>> {
    let params = new HttpParams();
    if (searchTerm) params = params.set('searchTerm', searchTerm);
    if (isActive !== undefined && isActive !== null) params = params.set('isActive', isActive.toString());

    return this.http.get<ApiResponse<AnnouncementRecord[]>>(this.apiUrl, { params });
  }

  getById(id: number): Observable<ApiResponse<AnnouncementRecord>> {
    return this.http.get<ApiResponse<AnnouncementRecord>>(`${this.apiUrl}/${id}`);
  }

  create(request: CreateAnnouncementRequest): Observable<ApiResponse<AnnouncementRecord>> {
    return this.http.post<ApiResponse<AnnouncementRecord>>(this.apiUrl, request);
  }

  update(id: number, request: UpdateAnnouncementRequest): Observable<ApiResponse<any>> {
    return this.http.put<ApiResponse<any>>(`${this.apiUrl}/${id}`, request);
  }

  delete(id: number): Observable<ApiResponse<any>> {
    return this.http.delete<ApiResponse<any>>(`${this.apiUrl}/${id}`);
  }

  toggleActive(id: number): Observable<ApiResponse<any>> {
    return this.http.put<ApiResponse<any>>(`${this.apiUrl}/${id}/toggle`, {});
  }
}
