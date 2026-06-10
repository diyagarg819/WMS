import { Injectable } from '@angular/core';
import { HttpRequest, HttpHandler, HttpEvent, HttpInterceptor } from '@angular/common/http';
import { Observable } from 'rxjs';

// This interceptor ensures that all HTTP requests send cookies.
// The JWT token is stored securely as an HttpOnly cookie by the browser.

@Injectable()
export class JwtInterceptor implements HttpInterceptor {

  constructor() {}

  intercept(request: HttpRequest<unknown>, next: HttpHandler): Observable<HttpEvent<unknown>> {

    // Clone the request and add withCredentials to ensure HttpOnly cookies are sent
    request = request.clone({
      withCredentials: true
    });

    return next.handle(request);
  }
}
