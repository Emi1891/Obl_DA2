import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import {
  Product,
  CreateProductRequest,
  UpdateProductRequest,
  ProductFilters,
  ImporterInfo,
  ImportProductsResponse,
  DateRange
} from '../models/product.model';
import { environment } from '../../../environments/environment';
import { emptyOn404 } from '../../shared/utils/http';

@Injectable({ providedIn: 'root' })
export class ProductService {
  private readonly http = inject(HttpClient);
  private readonly apiUrl = `${environment.apiUrl}/products`;

  getProducts(filters: ProductFilters = {}): Observable<Product[]> {
    let params = new HttpParams();
    if (filters.productLine) params = params.set('ProductLine', filters.productLine);
    if (filters.name) params = params.set('Name', filters.name);
    if (filters.categories?.length) {
      for (const c of filters.categories) params = params.append('Categories', c);
    }

    return this.http.get<Product[]>(this.apiUrl, { params }).pipe(emptyOn404());
  }

  createProduct(data: CreateProductRequest) {
    return this.http.post(this.apiUrl, data, { responseType: 'text' });
  }

  updateProduct(data: UpdateProductRequest) {
    return this.http.put(this.apiUrl, data, { responseType: 'text' });
  }

  updateStatus(id: number, isActive: boolean) {
    return this.http.patch(`${this.apiUrl}/${id}`, { isActive }, { responseType: 'text' });
  }

  getMostRequested(range: DateRange): Observable<Product[]> {
    const params = new HttpParams()
      .set('DateFrom', range.dateFrom)
      .set('DateTo', range.dateTo);

    return this.http.get<Product[]>(`${this.apiUrl}/most-requested`, { params }).pipe(emptyOn404());
  }

  getImporters(): Observable<ImporterInfo[]> {
    return this.http.get<ImporterInfo[]>(`${this.apiUrl}/importers`);
  }

  importProducts(importer: string, file: File): Observable<ImportProductsResponse> {
    const form = new FormData();
    form.append('importer', importer);
    form.append('file', file);
    return this.http.post<ImportProductsResponse>(`${this.apiUrl}/import`, form);
  }

  uploadImporter(file: File): Observable<{ message: string }> {
    const form = new FormData();
    form.append('file', file);
    return this.http.post<{ message: string }>(`${this.apiUrl}/importers`, form);
  }
}
