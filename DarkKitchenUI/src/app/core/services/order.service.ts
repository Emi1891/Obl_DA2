import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable, of, throwError } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { AuthService } from './auth.service';
import { SalesReport } from '../models/sales-report.model';
import {
  CreateOrderRequest,
  OrderResponse,
  OrderFilters,
  UpdateOrderStatusResponse
} from '../models/order.model';
import { environment } from '../../../environments/environment';

@Injectable({ providedIn: 'root' })
export class OrderService {
  private readonly http = inject(HttpClient);
  private readonly auth = inject(AuthService);
  private readonly apiUrl = `${environment.apiUrl}/orders`;

  getSalesReport(): Observable<SalesReport> {
    return this.http.get<SalesReport>(`${this.apiUrl}/sales-report`, {
      headers: this.auth.getAuthHeaders()
    });
  }

  createOrder(data: CreateOrderRequest): Observable<OrderResponse> {
    return this.http.post<OrderResponse>(this.apiUrl, data, {
      headers: this.auth.getAuthHeaders()
    });
  }

  getClientOrders(filters: OrderFilters = {}): Observable<OrderResponse[]> {
    let params = new HttpParams();
    if (filters.dateFrom) params = params.set('DateFrom', filters.dateFrom);
    if (filters.dateTo) params = params.set('DateTo', filters.dateTo);
    if (filters.status) params = params.set('Status', filters.status);

    return this.http.get<OrderResponse[]>(`${this.apiUrl}/client`, {
      headers: this.auth.getAuthHeaders(),
      params
    }).pipe(
      catchError(err => err?.status === 404 ? of([] as OrderResponse[]) : throwError(() => err))
    );
  }

  getOrdersByStatus(filters: OrderFilters): Observable<OrderResponse[]> {
    let params = new HttpParams();
    if (filters.dateFrom) params = params.set('DateFrom', filters.dateFrom);
    if (filters.dateTo) params = params.set('DateTo', filters.dateTo);
    if (filters.address) params = params.set('Address', filters.address);
    if (filters.status) params = params.set('Status', filters.status);

    return this.http.get<OrderResponse[]>(this.apiUrl, {
      headers: this.auth.getAuthHeaders(),
      params
    }).pipe(
      catchError(err => err?.status === 404 ? of([] as OrderResponse[]) : throwError(() => err))
    );
  }

  getOrderById(orderId: number): Observable<OrderResponse> {
    return this.http.get<OrderResponse>(`${this.apiUrl}/${orderId}`, {
      headers: this.auth.getAuthHeaders()
    });
  }

  updateOrderStatus(orderId: number, status: string): Observable<UpdateOrderStatusResponse> {
    return this.http.patch<UpdateOrderStatusResponse>(`${this.apiUrl}/${orderId}/status`, { status }, {
      headers: this.auth.getAuthHeaders()
    });
  }
}
