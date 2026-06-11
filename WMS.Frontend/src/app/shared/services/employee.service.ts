import { environment } from '../../../environments/environment';
import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { ApiResponse } from '../models/api-response.model';
import { Employee, EmployeeDto } from '../models/employee.model';

@Injectable({
  providedIn: 'root'
})
export class EmployeeService {
  private apiUrl = environment.apiUrl + '/employee';

  constructor(private http: HttpClient) { }

  getEmployees(search?: string): Observable<ApiResponse<Employee[]>> {
    let params = new HttpParams();
    if (search) {
      params = params.set('searchTerm', search);
    }
    
    return this.http.get<ApiResponse<Employee[]>>(this.apiUrl, { params });
  }

  getEmployeeById(id: number): Observable<ApiResponse<Employee>> {
    return this.http.get<ApiResponse<Employee>>(`${this.apiUrl}/${id}`);
  }

  createEmployee(employee: EmployeeDto): Observable<ApiResponse<Employee>> {
    return this.http.post<ApiResponse<Employee>>(this.apiUrl, employee);
  }

  updateEmployee(id: number, employee: EmployeeDto): Observable<ApiResponse<Employee>> {
    return this.http.put<ApiResponse<Employee>>(`${this.apiUrl}/${id}`, employee);
  }

  deleteEmployee(id: number): Observable<ApiResponse<boolean>> {
    return this.http.delete<ApiResponse<boolean>>(`${this.apiUrl}/${id}`);
  }
}
