import { Injectable } from '@angular/core';
import {
  HttpRequest,
  HttpHandler,
  HttpEvent,
  HttpInterceptor,
  HttpErrorResponse,
  HTTP_INTERCEPTORS
} from '@angular/common/http';
import { Observable, throwError } from 'rxjs';
import { catchError, delay } from 'rxjs/operators';

@Injectable()
export class ErrorInterceptor implements HttpInterceptor {

  constructor() {}

  intercept(request: HttpRequest<unknown>, next: HttpHandler): Observable<HttpEvent<unknown>> {
    return next.handle(request).pipe(
      catchError((error) => {
        if (error) {
        if (error.status === 401){
          return throwError(error.statusText);
        }
        if (error instanceof HttpErrorResponse){
          const applicationError = error.headers.get('Application-error');
          if (applicationError){
            return throwError(applicationError);
          }
          const serverError = error.error;
          let modalStateError = '';
          if (serverError.errors && typeof serverError.errors === 'object'){
            for (const key in serverError.errors){
              if (serverError.errors[key]) {
                modalStateError += serverError.errors[key] + '\n';
              }
            }
          }
          return throwError(modalStateError || serverError || 'Server Error');
        }}
      })
    );
  }
}
export const ErrorInterceptorProvider = {
  provide: HTTP_INTERCEPTORS,
  useClass: ErrorInterceptor,
  multi: true
};
