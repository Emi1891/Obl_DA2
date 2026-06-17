import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Router } from '@angular/router';
import { tap } from 'rxjs';
import { LoginRequest, LoginResponse } from '../models/user.model';
import { environment } from '../../../environments/environment';

@Injectable({ providedIn: 'root' })
export class AuthService {
  private readonly http = inject(HttpClient);
  private readonly router = inject(Router);
  private readonly apiUrl = environment.apiUrl;
  private readonly tokenKey = 'token';

  login(credentials: LoginRequest) {
    return this.http.post<LoginResponse>(`${this.apiUrl}/sessions`, credentials).pipe(
      tap(response => localStorage.setItem(this.tokenKey, response.token))
    );
  }

  logout(sessionExpired = false) {
    localStorage.removeItem(this.tokenKey);
    this.router.navigate(['/login'], sessionExpired ? { state: { sessionExpired: true } } : {});
  }

  getToken(): string | null {
    return localStorage.getItem(this.tokenKey);
  }

  getPermissions(): string[] {
    const token = this.getToken();
    if (!token) return [];
    try {
      const payload = JSON.parse(atob(token.split('.')[1]));
      const perms = payload['permission'];
      if (!perms) return [];
      return Array.isArray(perms) ? perms : [perms];
    } catch {
      return [];
    }
  }

  private cachedToken: string | null = null;
  private cachedPermissions = new Set<string>();

  private permissionSet(): Set<string> {
    const token = this.getToken();
    if (token !== this.cachedToken) {
      this.cachedToken = token;
      this.cachedPermissions = new Set(this.getPermissions());
    }
    return this.cachedPermissions;
  }

  hasPermission(permission: string): boolean {
    return this.permissionSet().has(permission);
  }

  hasAnyPermission(permissions: string[]): boolean {
    const set = this.permissionSet();
    return permissions.some(p => set.has(p));
  }

  isTokenExpired(): boolean {
    const token = this.getToken();
    if (!token) return true;
    try {
      const payload = JSON.parse(atob(token.split('.')[1]));
      return payload['exp'] * 1000 < Date.now();
    } catch {
      return true;
    }
  }

  getUserName(): string | null {
    const token = this.getToken();
    if (!token) return null;
    try {
      const payload = JSON.parse(atob(token.split('.')[1]));
      return payload['name'] ?? payload['unique_name'] ?? payload['email'] ?? null;
    } catch {
      return null;
    }
  }
}
