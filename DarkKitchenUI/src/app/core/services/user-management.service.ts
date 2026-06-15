import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable, of, throwError } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { AuthService } from './auth.service';
import {
  UserListItem,
  CreateUserPayload,
  UpdateUserPayload,
  UserFilters
} from '../models/user-management.model';
import { environment } from '../../../environments/environment';

@Injectable({ providedIn: 'root' })
export class UserManagementService {
  private readonly http = inject(HttpClient);
  private readonly auth = inject(AuthService);
  private readonly apiUrl = `${environment.apiUrl}/users`;

  getUsers(filters: UserFilters = {}): Observable<UserListItem[]> {
    let params = new HttpParams();
    if (filters.name) params = params.set('Name', filters.name);
    if (filters.surname) params = params.set('Surname', filters.surname);

    return this.http.get<UserListItem[]>(this.apiUrl, {
      headers: this.auth.getAuthHeaders(),
      params
    }).pipe(
      catchError(err => err?.status === 404 ? of([] as UserListItem[]) : throwError(() => err))
    );
  }

  createUser(payload: CreateUserPayload) {
    return this.http.post(`${this.apiUrl}/admin`, payload, {
      headers: this.auth.getAuthHeaders(),
      responseType: 'text'
    });
  }

  updateUser(payload: UpdateUserPayload) {
    return this.http.put(this.apiUrl, payload, {
      headers: this.auth.getAuthHeaders(),
      responseType: 'text'
    });
  }

  deleteUser(email: string) {
    return this.http.delete(`${this.apiUrl}/${encodeURIComponent(email)}`, {
      headers: this.auth.getAuthHeaders(),
      responseType: 'text'
    });
  }
}
