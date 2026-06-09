import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable, of, throwError } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { AuthService } from './auth.service';
import { AuditRecord, AuditFilter } from '../models/audit.model';

@Injectable({ providedIn: 'root' })
export class AuditService {
  private readonly http = inject(HttpClient);
  private readonly auth = inject(AuthService);
  private readonly apiUrl = 'https://localhost:7134/api/audit';

  getAuditRecords(filter: AuditFilter): Observable<AuditRecord[]> {
    let params = new HttpParams()
      .set('EntityName', filter.entityName)
      .set('DateFrom', filter.dateFrom)
      .set('DateTo', filter.dateTo);
    if (filter.entityId != null) {
      params = params.set('EntityId', filter.entityId);
    }

    return this.http.get<AuditRecord[]>(this.apiUrl, {
      headers: this.auth.getAuthHeaders(),
      params
    }).pipe(
      catchError(err => err?.status === 404 ? of([] as AuditRecord[]) : throwError(() => err))
    );
  }
}
