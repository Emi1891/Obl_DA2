import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, of, throwError } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { AuthService } from './auth.service';
import { ShippingType, ShippingTypeRequest } from '../models/order.model';
import { environment } from '../../../environments/environment';

@Injectable({ providedIn: 'root' })
export class ShippingTypeService {
  private readonly http = inject(HttpClient);
  private readonly auth = inject(AuthService);
  private readonly apiUrl = `${environment.apiUrl}/shippingtypes`;

  getShippingTypes(): Observable<ShippingType[]> {
    return this.http.get<ShippingType[]>(this.apiUrl, {
      headers: this.auth.getAuthHeaders()
    }).pipe(
      catchError(err => err?.status === 404 ? of([] as ShippingType[]) : throwError(() => err))
    );
  }

  createShippingType(payload: ShippingTypeRequest): Observable<ShippingType> {
    return this.http.post<ShippingType>(this.apiUrl, payload, {
      headers: this.auth.getAuthHeaders()
    });
  }

  updateShippingType(id: number, payload: ShippingTypeRequest): Observable<ShippingType> {
    return this.http.put<ShippingType>(`${this.apiUrl}/${id}`, payload, {
      headers: this.auth.getAuthHeaders()
    });
  }
}
