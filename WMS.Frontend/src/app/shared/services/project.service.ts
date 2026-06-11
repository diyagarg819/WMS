import { environment } from '../../../environments/environment';
import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { ApiResponse } from '../models/api-response.model';
import { 
  ProjectRecord, 
  CreateProjectRequest, 
  UpdateProjectRequest, 
  AssignEmployeeRequest, 
  ProjectAllocation
} from '../models/project.model';

@Injectable({
  providedIn: 'root'
})
export class ProjectService {
  private apiUrl = environment.apiUrl + '/project';

  constructor(private http: HttpClient) {}

  getAllProjects(searchTerm: string = ''): Observable<ApiResponse<ProjectRecord[]>> {
    let params = new HttpParams();
    if (searchTerm) {
      params = params.set('searchTerm', searchTerm);
    }

    return this.http.get<ApiResponse<ProjectRecord[]>>(this.apiUrl, { params });
  }

  getProjectById(id: number): Observable<ApiResponse<ProjectRecord>> {
    return this.http.get<ApiResponse<ProjectRecord>>(`${this.apiUrl}/${id}`);
  }

  createProject(request: CreateProjectRequest): Observable<ApiResponse<ProjectRecord>> {
    return this.http.post<ApiResponse<ProjectRecord>>(this.apiUrl, request);
  }

  updateProject(id: number, request: UpdateProjectRequest): Observable<ApiResponse<ProjectRecord>> {
    return this.http.put<ApiResponse<ProjectRecord>>(`${this.apiUrl}/${id}`, request);
  }

  deleteProject(id: number): Observable<ApiResponse<any>> {
    return this.http.delete<ApiResponse<any>>(`${this.apiUrl}/${id}`);
  }

  assignEmployee(projectId: number, request: AssignEmployeeRequest): Observable<ApiResponse<ProjectAllocation>> {
    request.projectId = projectId;
    return this.http.post<ApiResponse<ProjectAllocation>>(`${environment.apiUrl}/ProjectAllocation`, request);
  }

  removeEmployee(allocationId: number): Observable<ApiResponse<any>> {
    return this.http.put<ApiResponse<any>>(`${environment.apiUrl}/ProjectAllocation/remove/${allocationId}`, {});
  }


  getAllocationHistory(): Observable<ApiResponse<ProjectAllocation[]>> {
    return this.http.get<ApiResponse<ProjectAllocation[]>>(`${environment.apiUrl}/ProjectAllocation/history`);
  }

  getProjectsByEmployee(employeeId: number): Observable<ApiResponse<ProjectRecord[]>> {
    return this.http.get<ApiResponse<ProjectRecord[]>>(`${environment.apiUrl}/employees/${employeeId}/projects`);
  }

  getEmployeesByProject(projectId: number): Observable<ApiResponse<ProjectAllocation[]>> {
    return this.http.get<ApiResponse<ProjectAllocation[]>>(`${this.apiUrl}/${projectId}/employees`);
  }
}
