import { Injectable } from '@angular/core';
import { CanActivate, Router } from '@angular/router';
import { AuthService } from '../services/auth.service';

// This guard protects routes that require authentication.
// If the user is not logged in (no token in localStorage), they get redirected to the login page.
// This also handles the "back button after logout" scenario — since the token is cleared
// on logout, any attempt to navigate to a protected route will fail this check.

@Injectable({
  providedIn: 'root'
})
export class AuthGuard implements CanActivate {

  constructor(private authService: AuthService, private router: Router) {}

  canActivate(): boolean {
    // Simple check: is there a token in localStorage?
    if (this.authService.isLoggedIn()) {
      return true;
    }

    // No token — redirect to login page
    this.router.navigate(['/auth/login']);
    return false;
  }
}
