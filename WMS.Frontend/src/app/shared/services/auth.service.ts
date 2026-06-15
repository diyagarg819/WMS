import { environment } from '../../../environments/environment';
import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, BehaviorSubject, tap } from 'rxjs';
import { Role } from '../enums/role.enum';
import { Router } from '@angular/router';

@Injectable({
  providedIn: 'root'
})
export class AuthService {

  private apiUrl = environment.apiUrl + '/auth';

  private isAuthenticatedSubject = new BehaviorSubject<boolean>(this.hasToken());
  public isAuthenticated$ = this.isAuthenticatedSubject.asObservable();

  constructor(private http: HttpClient, private router: Router) { }

  private hasToken(): boolean {
    return !!localStorage.getItem('token');
  }

  // Handle Login
  login(credentials: any): Observable<any> {
    return this.http.post(`${this.apiUrl}/login`, credentials)
      .pipe(
        tap((response: any) => {
          if (response.success && response.data) {
            localStorage.setItem('token', response.data.accessToken);
            localStorage.setItem('userRole', response.data.role);
            this.isAuthenticatedSubject.next(true);
          }
        })
      );
  }

  // Handle Logout
  logout(): void {
    // We don't necessarily need to call the backend if we are just dropping the token locally,
    // but we can do a fire-and-forget if we want. For simplicity, we just clear local state.
    localStorage.removeItem('token');
    localStorage.removeItem('userRole');
    this.isAuthenticatedSubject.next(false);
    this.router.navigate(['/auth/login']);
  }

  // Check if the user is currently logged in
  isLoggedIn(): boolean {
    return this.hasToken();
  }

  // Get the role from localStorage
  getRole(): Role | null {
    const roleStr = localStorage.getItem('userRole');
    if (!roleStr) return null;
    return roleStr as Role;
  }

  // Get Username from token
  getUsername(): string {
    const token = localStorage.getItem('token');
    if (!token) return 'User';
    try {
      const payload = JSON.parse(atob(token.split('.')[1]));
      return payload['http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name'] || 
             payload.name || 
             payload.email || 
             'User';
    } catch (e) {
      return 'User';
    }
  }

  // Get UserId from token
  getUserId(): number | null {
    const token = localStorage.getItem('token');
    if (!token) return null;
    try {
      const payload = JSON.parse(atob(token.split('.')[1]));
      // nameid claim contains UserId
      const userIdStr = payload['http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier'];
      return userIdStr ? parseInt(userIdStr, 10) : null;
    } catch (e) {
      return null;
    }
  }
}
