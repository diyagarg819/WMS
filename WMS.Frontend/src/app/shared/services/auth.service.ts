import { environment } from '../../../environments/environment';
import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, throwError, of } from 'rxjs';
import { catchError, tap, map } from 'rxjs/operators';
import { ApiResponse } from '../models/api-response.model';
import { Role } from '../enums/role.enum';

@Injectable({
  providedIn: 'root'
})
export class AuthService {

  private apiUrl = environment.apiUrl + '/auth';

  // We no longer store the JWT in localStorage for security (HttpOnly Cookie is used instead).
  // We only store a flag and the user's role to manage UI state synchronously.
  private readonly AUTH_FLAG_KEY = 'isAuthenticated';
  private readonly ROLE_KEY = 'userRole';

  constructor(private http: HttpClient) { }

  // ── Login ─────────────────────────────────────────────────────────────
  // Send username + password to the backend.
  // The backend sets the HttpOnly cookie. We just save the UI state locally.
  login(credentials: any): Observable<ApiResponse<any>> {
    // Add withCredentials to ensure we receive the cookie in the response
    return this.http.post<ApiResponse<any>>(`${this.apiUrl}/login`, credentials, { withCredentials: true }).pipe(
      tap(res => {
        if (res.success && res.data) {
          localStorage.setItem(this.AUTH_FLAG_KEY, 'true');
          if (res.data.role) {
            localStorage.setItem(this.ROLE_KEY, res.data.role);
          }
        }
      })
    );
  }

  // ── Logout ────────────────────────────────────────────────────────────
  // Tell the backend we're logging out (clears cookie), then clear local state.
  logout(): Observable<any> {
    return this.http.post<ApiResponse<any>>(`${this.apiUrl}/logout`, {}, { withCredentials: true }).pipe(
      tap(() => this.clearLocalState()),
      catchError(err => {
        this.clearLocalState();
        return throwError(() => err);
      })
    );
  }

  // ── Auth Verification ──────────────────────────────────────────────────
  // Calls the /me endpoint to verify if the HttpOnly cookie is still valid.
  verifySession(): Observable<boolean> {
    return this.http.get<ApiResponse<any>>(`${this.apiUrl}/me`, { withCredentials: true }).pipe(
      map(res => {
        if (res.success) {
          localStorage.setItem(this.AUTH_FLAG_KEY, 'true');
          if (res.data && res.data.role) {
            localStorage.setItem(this.ROLE_KEY, res.data.role);
          }
          return true;
        }
        this.clearLocalState();
        return false;
      }),
      catchError(() => {
        this.clearLocalState();
        return of(false);
      })
    );
  }

  // ── Sync State Helpers ────────────────────────────────────────────────

  // Check if the user is currently logged in (based on UI flag)
  isLoggedIn(): boolean {
    return localStorage.getItem(this.AUTH_FLAG_KEY) === 'true';
  }

  // Remove the UI state
  clearLocalState(): void {
    localStorage.removeItem(this.AUTH_FLAG_KEY);
    localStorage.removeItem(this.ROLE_KEY);
  }

  // ── Role Extraction ───────────────────────────────────────────────────
  // Get the role from localStorage since we can no longer decode the HttpOnly JWT.
  getRole(): Role | null {
    const roleStr = localStorage.getItem(this.ROLE_KEY);
    if (!roleStr) return null;
    return roleStr as Role;
  }
}
