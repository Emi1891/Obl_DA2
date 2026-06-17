export interface Address {
  street: string;
  doorNumber: string;
  apartment?: string | null;
}

export interface OrderProductItem {
  productCode: string;
  quantity: number;
}

export interface CreateOrderRequest {
  shippingTypeId: number;
  address: Address;
  products: OrderProductItem[];
}

export interface OrderResponse {
  id: number;
  clientId: number;
  status: string;
  createdAt: string;
  subtotal: number;
  discount: number;
  iva: number;
  shippingCost: number;
  total: number;
  products: OrderProductItem[];
}

export interface ShippingType {
  id: number;
  name: string;
  price: number;
}

export interface ShippingTypeRequest {
  name: string;
  price: number;
}

export interface OrderFilters {
  dateFrom?: string;
  dateTo?: string;
  status?: string;
  address?: string;
}

export interface UpdateOrderStatusResponse {
  message: string;
  updatedAt: string;
}
