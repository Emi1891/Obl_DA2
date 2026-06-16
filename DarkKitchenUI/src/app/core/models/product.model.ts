export interface Product {
  id: number;
  code: string;
  name: string;
  description?: string | null;
  productLine?: string | null;
  category?: string | null;
  price: number;
  imageUrl?: string[] | null;
  isActive: boolean;
  unitsSold: number;
  discountPercentage?: number | null;
}

export function discountedUnitPrice(product: Product): number {
  return product.price * (1 - (product.discountPercentage ?? 0) / 100);
}

export interface CreateProductRequest {
  code: string;
  name: string;
  description?: string | null;
  productLine?: string | null;
  category?: string | null;
  price: number;
  imageUrl: string[];
}

export interface UpdateProductRequest {
  id: number;
  code?: string | null;
  name?: string | null;
  description?: string | null;
  productLine?: string | null;
  category?: string | null;
  price?: number | null;
  imageUrl?: string[] | null;
  isActive?: boolean | null;
}

export interface ProductFilters {
  productLine?: string;
  categories?: string[];
  name?: string;
}

export interface ImporterInfo {
  name: string;
  extension: string;
}

export interface ImportProductsResponse {
  imported: number;
  message: string;
}

export interface DateRange {
  dateFrom: string;
  dateTo: string;
}
