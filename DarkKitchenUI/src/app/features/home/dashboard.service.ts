import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, of } from 'rxjs';
import { map, catchError } from 'rxjs/operators';
import { AuthService } from '../../core/services/auth.service';

@Injectable({ providedIn: 'root' })
export class DashboardService {
  private readonly http = inject(HttpClient);
  private readonly auth = inject(AuthService);
  private readonly apiUrl = 'https://localhost:7134/api';

  private readonly dateFrom = '2000-01-01T00:00:00';
  private readonly dateTo = new Date(Date.now() + 365 * 24 * 60 * 60 * 1000).toISOString();

  private emptyAs0(err: unknown): Observable<number | null> {
    const status = (err as { status?: number })?.status;
    return status === 404 ? of(0) : of(null);
  }

  getOrderCountByStatus(status: string): Observable<number | null> {
    return this.http.get<unknown[]>(`${this.apiUrl}/orders`, {
      headers: this.auth.getAuthHeaders(),
      params: { Status: status, DateFrom: this.dateFrom, DateTo: this.dateTo }
    }).pipe(
      map(o => o.length),
      catchError(err => this.emptyAs0(err))
    );
  }

  getClientOrderCount(): Observable<number | null> {
    return this.http.get<unknown[]>(`${this.apiUrl}/orders/client`, {
      headers: this.auth.getAuthHeaders()
    }).pipe(
      map(o => o.length),
      catchError(err => this.emptyAs0(err))
    );
  }

  getProductCount(activeOnly = false): Observable<number | null> {
    return this.http.get<{ isActive: boolean }[]>(`${this.apiUrl}/products`, {
      headers: this.auth.getAuthHeaders()
    }).pipe(
      map(p => activeOnly ? p.filter(x => x.isActive).length : p.length),
      catchError(err => this.emptyAs0(err))
    );
  }

  getUserCount(): Observable<number | null> {
    return this.http.get<unknown[]>(`${this.apiUrl}/users`, {
      headers: this.auth.getAuthHeaders()
    }).pipe(
      map(u => u.length),
      catchError(err => this.emptyAs0(err))
    );
  }

  getTotalRevenue(): Observable<number | null> {
    return this.http.get<{ total: number }>(`${this.apiUrl}/orders/sales-report`, {
      headers: this.auth.getAuthHeaders()
    }).pipe(
      map(r => r.total),
      catchError(err => this.emptyAs0(err))
    );
  }

  getPromotionCount(): Observable<number | null> {
    const today = new Date().toISOString().split('T')[0];
    return this.http.get<unknown[]>(`${this.apiUrl}/promotions`, {
      headers: this.auth.getAuthHeaders(),
      params: { Date: today }
    }).pipe(
      map(p => p.length),
      catchError(err => this.emptyAs0(err))
    );
  }
}
