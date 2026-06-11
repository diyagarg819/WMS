import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { ApiResponse } from '../models/api-response.model';
import { ClientRecord, CreateClientRequest, UpdateClientRequest } from '../models/client.model';

@Injectable({
  providedIn: 'root'
})
export class ClientService {
  private apiUrl = `${environment.apiUrl}/Client`;

  constructor(private http: HttpClient) {}

  getAllClients(searchTerm: string = ''): Observable<ApiResponse<ClientRecord[]>> {
    let params = new HttpParams();
    if (searchTerm) {
      params = params.set('searchTerm', searchTerm);
    }
    return this.http.get<ApiResponse<ClientRecord[]>>(this.apiUrl, { params });
  }

  getClientById(id: number): Observable<ApiResponse<ClientRecord>> {
    return this.http.get<ApiResponse<ClientRecord>>(`${this.apiUrl}/${id}`);
  }

  createClient(data: CreateClientRequest): Observable<ApiResponse<ClientRecord>> {
    return this.http.post<ApiResponse<ClientRecord>>(this.apiUrl, data);
  }

  updateClient(id: number, data: UpdateClientRequest): Observable<ApiResponse<ClientRecord>> {
    return this.http.put<ApiResponse<ClientRecord>>(`${this.apiUrl}/${id}`, data);
  }

  deleteClient(id: number): Observable<ApiResponse<any>> {
    return this.http.delete<ApiResponse<any>>(`${this.apiUrl}/${id}`);
  }
}
