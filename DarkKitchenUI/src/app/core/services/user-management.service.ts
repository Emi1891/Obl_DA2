import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import {
  UserListItem,
  CreateUserPayload,
  UpdateUserPayload,
  UserFilters
} from '../models/user-management.model';
import { environment } from '../../../environments/environment';
import { emptyOn404 } from '../../shared/utils/http';

@Injectable({ providedIn: 'root' })
export class UserManagementService {
  private readonly http = inject(HttpClient);
  private readonly apiUrl = `${environment.apiUrl}/users`;

  getUsers(filters: UserFilters = {}): Observable<UserListItem[]> {
    let params = new HttpParams();
    if (filters.name) params = params.set('Name', filters.name);
    if (filters.surname) params = params.set('Surname', filters.surname);

    return this.http.get<UserListItem[]>(this.apiUrl, { params }).pipe(emptyOn404());
  }

  createUser(payload: CreateUserPayload) {
    return this.http.post(`${this.apiUrl}/admin`, payload, { responseType: 'text' });
  }

  updateUser(payload: UpdateUserPayload) {
    return this.http.put(this.apiUrl, payload, { responseType: 'text' });
  }

  deleteUser(email: string) {
    return this.http.delete(`${this.apiUrl}/${encodeURIComponent(email)}`, { responseType: 'text' });
  }
}
