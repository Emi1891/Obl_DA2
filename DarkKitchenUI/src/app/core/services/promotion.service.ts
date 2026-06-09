import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable, of, throwError } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { AuthService } from './auth.service';
import { Promotion, PromotionFilters, PromotionRequest } from '../models/promotion.model';

@Injectable({ providedIn: 'root' })
export class PromotionService {
  private readonly http = inject(HttpClient);
  private readonly auth = inject(AuthService);
  private readonly apiUrl = 'https://localhost:7134/api/promotions';

  getAll(filters: PromotionFilters = {}): Observable<Promotion[]> {
    let params = new HttpParams();
    if (filters.date) params = params.set('Date', filters.date);
    if (filters.productLine) params = params.set('ProductLine', filters.productLine);
    if (filters.productName) params = params.set('ProductName', filters.productName);

    return this.http.get<Promotion[]>(this.apiUrl, {
      headers: this.auth.getAuthHeaders(),
      params
    }).pipe(
      catchError(err => err?.status === 404 ? of([] as Promotion[]) : throwError(() => err))
    );
  }

  create(data: PromotionRequest) {
    return this.http.post(this.apiUrl, data, {
      headers: this.auth.getAuthHeaders(),
      responseType: 'text'
    });
  }

  update(id: number, data: PromotionRequest) {
    return this.http.put(`${this.apiUrl}/${id}`, data, {
      headers: this.auth.getAuthHeaders(),
      responseType: 'text'
    });
  }

  updateProducts(id: number, productIds: number[]) {
    return this.http.put(`${this.apiUrl}/${id}/products`, { products: productIds }, {
      headers: this.auth.getAuthHeaders(),
      responseType: 'text'
    });
  }
}
