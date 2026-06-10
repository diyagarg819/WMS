import { environment } from '../../../environments/environment';
import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { ApiResponse, PagedData } from '../models/api-response.model';
import { 
  ProjectRecord, 
  CreateProjectRequest, 
  UpdateProjectRequest, 
  AssignEmployeeRequest, 
  UpdateAllocationStatusRequest,
  ProjectAllocation
} from '../models/project.model';

@Injectable({
  providedIn: 'root'
})
export class ProjectService {
  private apiUrl = environment.apiUrl + '/project';

  constructor(private http: HttpClient) {}

  getAllProjects(pageNumber: number, pageSize: number, searchTerm?: string): Observable<ApiResponse<PagedData<ProjectRecord>>> {
    let params = new HttpParams()
      .set('pageNumber', pageNumber.toString())
      .set('pageSize', pageSize.toString());
      
    if (searchTerm) {
      params = params.set('searchTerm', searchTerm);
    }

    return this.http.get<ApiResponse<PagedData<ProjectRecord>>>(this.apiUrl, { params });
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
    return this.http.post<ApiResponse<ProjectAllocation>>(`${this.apiUrl}/${projectId}/assign`, request);
  }

  removeEmployee(allocationId: number): Observable<ApiResponse<any>> {
    return this.http.post<ApiResponse<any>>(`${this.apiUrl}/allocation/${allocationId}/remove`, {});
  }

  updateAllocationStatus(allocationId: number, request: UpdateAllocationStatusRequest): Observable<ApiResponse<ProjectAllocation>> {
    return this.http.put<ApiResponse<ProjectAllocation>>(`${this.apiUrl}/allocation/${allocationId}/status`, request);
  }
}
