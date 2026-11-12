import { HttpInterceptorFn } from '@angular/common/http';
import { environment } from '../../../environments/environment';

export const apiBaseInterceptor: HttpInterceptorFn = (req, next) => {
  const base = environment.apiBaseUrl?.replace(/\/+$/, '');
  if (base && req.url.startsWith('/api')) {
    req = req.clone({
      url: `${base}${req.url}`
    });
  }
  return next(req);
};
