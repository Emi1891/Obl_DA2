import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { AuthService } from '../services/auth.service';

export function permissionGuard(required: string | string[]): CanActivateFn {
  return () => {
    const auth = inject(AuthService);
    const router = inject(Router);
    const perms = new Set(auth.getPermissions());
    const allowed = Array.isArray(required)
      ? required.some(p => perms.has(p))
      : perms.has(required);
    if (allowed) return true;
    return router.createUrlTree(['/home']);
  };
}
