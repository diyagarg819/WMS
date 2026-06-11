import { Injectable } from '@angular/core';
import { HttpRequest, HttpHandler, HttpEvent, HttpInterceptor, HttpErrorResponse } from '@angular/common/http';
import { Observable, throwError } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { MatDialog } from '@angular/material/dialog';
import { ErrorDialogComponent } from '../components/error-dialog/error-dialog.component';
import { AuthService } from '../services/auth.service';
import { Router } from '@angular/router';

@Injectable()
export class ErrorInterceptor implements HttpInterceptor {

  constructor(private dialog: MatDialog, private authService: AuthService, private router: Router) {}

  intercept(request: HttpRequest<unknown>, next: HttpHandler): Observable<HttpEvent<unknown>> {
    return next.handle(request).pipe(
      catchError((error: HttpErrorResponse) => {
        let errorMsg = '';
        let title = 'Error';
        let isBackendDown = false;

        if (error.error instanceof ErrorEvent) {
          // Client-side error
          errorMsg = `Client Error: ${error.error.message}`;
        } else {
          // Server-side error
          if (error.status === 0) {
            title = 'Connection Failed';
            errorMsg = 'Cannot communicate with the server. It might be offline or blocked by CORS.';
            isBackendDown = true;
          } else if (error.status === 401) {
            // Unauthorized — cookie expired or invalid. Clear state and redirect to login.
            // But don't redirect if we are already on the login page (to avoid loops)
            if (!this.router.url.includes('/auth/login')) {
                this.authService.logout();
            }
            return throwError(() => error);
          } else if (error.status === 400 || error.status === 403) {
            // Do not show the global error dialog for validation or forbidden errors.
            // Let the component handle it.
            return throwError(() => error);
          } else {
            errorMsg = error.error?.message || `Server Error: ${error.status} ${error.statusText}`;
          }
        }

        // Open the Error Dialog
        this.dialog.open(ErrorDialogComponent, {
          width: '400px',
          disableClose: false,
          data: {
            title: title,
            message: errorMsg,
            isBackendDown: isBackendDown
          }
        });

        return throwError(() => error);
      })
    );
  }
}
