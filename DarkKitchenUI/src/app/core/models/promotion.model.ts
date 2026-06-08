export interface Promotion {
  id: number;
  name: string;
  discountPercentage: number;
  dateFrom: string;
  dateTo: string;
  productCodes: string[];
}

export interface PromotionRequest {
  name: string;
  discountPercentage: number;
  dateFrom: string;
  dateTo: string;
}

export interface PromotionFilters {
  date?: string;
  productLine?: string;
  productName?: string;
}
