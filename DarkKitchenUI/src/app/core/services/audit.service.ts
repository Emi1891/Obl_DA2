import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { AuditRecord, AuditFilter } from '../models/audit.model';
import { environment } from '../../../environments/environment';
import { emptyOn404 } from '../../shared/utils/http';

@Injectable({ providedIn: 'root' })
export class AuditService {
  private readonly http = inject(HttpClient);
  private readonly apiUrl = `${environment.apiUrl}/audit`;

  getAuditRecords(filter: AuditFilter): Observable<AuditRecord[]> {
    let params = new HttpParams()
      .set('EntityName', filter.entityName)
      .set('DateFrom', filter.dateFrom)
      .set('DateTo', filter.dateTo);
    if (filter.entityId != null) {
      params = params.set('EntityId', filter.entityId);
    }

    return this.http.get<AuditRecord[]>(this.apiUrl, { params }).pipe(emptyOn404());
  }
}
