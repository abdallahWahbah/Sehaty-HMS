import { Injectable } from '@angular/core';
import { HttpErrorResponse, HttpEvent, HttpHandler, HttpInterceptor, HttpRequest } from '@angular/common/http';
import { Observable, catchError, switchMap, throwError } from 'rxjs';
import { AuthService } from '../services/auth.service';

@Injectable()
export class TokenInterceptor implements HttpInterceptor {

  private isRefreshing = false;

  constructor(private authService: AuthService) {}

  intercept(req: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
    
    const token = localStorage.getItem('token');

    let authReq = req;

    if (token) {
      authReq = req.clone({
        setHeaders: { Authorization: `Bearer ${token}` }
      });
    }

    return next.handle(authReq).pipe(
      catchError((err: HttpErrorResponse) => {

        // If token expired → 401
        if (err.status === 401 && !this.isRefreshing) {
          this.isRefreshing = true;

          return this.authService.refreshToken().pipe(
            switchMap((data) => {
              this.isRefreshing = false;

              // store new tokens
              localStorage.setItem("token", data.token);
              localStorage.setItem("refreshToken", data.refreshToken);

              // retry original request
              const newReq = req.clone({
                setHeaders: { Authorization: `Bearer ${data.token}` }
              });

              return next.handle(newReq);
            }),
            catchError(refreshError => {
              this.isRefreshing = false;
              localStorage.clear();
              // optionally redirect to login
              return throwError(() => refreshError);
            })
          );
        }

        return throwError(() => err);
      })
    );
  }
}