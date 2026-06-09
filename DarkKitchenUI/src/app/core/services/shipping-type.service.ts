import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, of, throwError } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { AuthService } from './auth.service';
import { ShippingType } from '../models/order.model';

@Injectable({ providedIn: 'root' })
export class ShippingTypeService {
  private readonly http = inject(HttpClient);
  private readonly auth = inject(AuthService);
  private readonly apiUrl = 'https://localhost:7134/api/shippingtypes';

  getShippingTypes(): Observable<ShippingType[]> {
    return this.http.get<ShippingType[]>(this.apiUrl, {
      headers: this.auth.getAuthHeaders()
    }).pipe(
      catchError(err => err?.status === 404 ? of([] as ShippingType[]) : throwError(() => err))
    );
  }
}
