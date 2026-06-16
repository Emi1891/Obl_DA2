import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { ShippingType, ShippingTypeRequest } from '../models/order.model';
import { environment } from '../../../environments/environment';
import { emptyOn404 } from '../../shared/utils/http';

@Injectable({ providedIn: 'root' })
export class ShippingTypeService {
  private readonly http = inject(HttpClient);
  private readonly apiUrl = `${environment.apiUrl}/shippingtypes`;

  getShippingTypes(): Observable<ShippingType[]> {
    return this.http.get<ShippingType[]>(this.apiUrl).pipe(emptyOn404());
  }

  createShippingType(payload: ShippingTypeRequest): Observable<ShippingType> {
    return this.http.post<ShippingType>(this.apiUrl, payload);
  }

  updateShippingType(id: number, payload: ShippingTypeRequest): Observable<ShippingType> {
    return this.http.put<ShippingType>(`${this.apiUrl}/${id}`, payload);
  }
}
