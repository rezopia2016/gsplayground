import { HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { from, switchMap } from 'rxjs';
import { AuthService } from '../services/auth.service';

/** Attaches the Azure AD bearer token to every API call so backend RBAC policies apply (UR-062). */
export const authInterceptor: HttpInterceptorFn = (req, next) => {
  const authService = inject(AuthService);

  return from(authService.acquireApiToken()).pipe(
    switchMap((token) => {
      const authorizedRequest = token ? req.clone({ setHeaders: { Authorization: `Bearer ${token}` } }) : req;
      return next(authorizedRequest);
    })
  );
};
