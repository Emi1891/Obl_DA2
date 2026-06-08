import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { AuthService } from './auth.service';
import { SalesReport } from '../models/sales-report.model';
import { CreateOrderRequest, OrderResponse } from '../models/order.model';

@Injectable({ providedIn: 'root' })
export class OrderService {
  private readonly http = inject(HttpClient);
  private readonly auth = inject(AuthService);
  private readonly apiUrl = 'https://localhost:7134/api/orders';

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
}
