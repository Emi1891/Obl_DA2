import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Promotion, PromotionFilters, PromotionRequest } from '../models/promotion.model';
import { environment } from '../../../environments/environment';
import { emptyOn404 } from '../../shared/utils/http';

@Injectable({ providedIn: 'root' })
export class PromotionService {
  private readonly http = inject(HttpClient);
  private readonly apiUrl = `${environment.apiUrl}/promotions`;

  getAll(filters: PromotionFilters = {}): Observable<Promotion[]> {
    let params = new HttpParams();
    if (filters.date) params = params.set('Date', filters.date);
    if (filters.productLine) params = params.set('ProductLine', filters.productLine);
    if (filters.productName) params = params.set('ProductName', filters.productName);

    return this.http.get<Promotion[]>(this.apiUrl, { params }).pipe(emptyOn404());
  }

  create(data: PromotionRequest) {
    return this.http.post(this.apiUrl, data, { responseType: 'text' });
  }

  update(id: number, data: PromotionRequest) {
    return this.http.put(`${this.apiUrl}/${id}`, data, { responseType: 'text' });
  }

  updateProducts(id: number, productIds: number[]) {
    return this.http.put(`${this.apiUrl}/${id}/products`, { products: productIds }, { responseType: 'text' });
  }
}
