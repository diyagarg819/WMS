import { environment } from '../../../environments/environment';
import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { ApiResponse } from '../models/api-response.model';
import { Department, CreateDepartmentRequest, UpdateDepartmentRequest } from '../models/department.model';

@Injectable({
  providedIn: 'root'
})
export class DepartmentService {
  private apiUrl = environment.apiUrl + '/department';

  constructor(private http: HttpClient) {}

  getAllDepartments(searchTerm?: string): Observable<ApiResponse<Department[]>> {
    let params = new HttpParams();
    if (searchTerm) {
      params = params.set('searchTerm', searchTerm);
    }

    return this.http.get<ApiResponse<Department[]>>(this.apiUrl, { params });
  }

  getDepartmentById(id: number): Observable<ApiResponse<Department>> {
    return this.http.get<ApiResponse<Department>>(`${this.apiUrl}/${id}`);
  }

  createDepartment(request: CreateDepartmentRequest): Observable<ApiResponse<Department>> {
    return this.http.post<ApiResponse<Department>>(this.apiUrl, request);
  }

  updateDepartment(id: number, request: UpdateDepartmentRequest): Observable<ApiResponse<Department>> {
    return this.http.put<ApiResponse<Department>>(`${this.apiUrl}/${id}`, request);
  }

  deleteDepartment(id: number): Observable<ApiResponse<any>> {
    return this.http.delete<ApiResponse<any>>(`${this.apiUrl}/${id}`);
  }
}
