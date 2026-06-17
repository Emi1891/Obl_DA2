import { Injectable, computed, signal } from '@angular/core';
import { Product, discountedUnitPrice } from '../models/product.model';

export interface CartLine {
  product: Product;
  quantity: number;
}

@Injectable({ providedIn: 'root' })
export class CartService {
  private readonly lines = signal<CartLine[]>([]);

  readonly items = this.lines.asReadonly();
  readonly count = computed(() => this.lines().reduce((sum, l) => sum + l.quantity, 0));
  readonly subtotal = computed(() => this.lines().reduce((sum, l) => sum + l.product.price * l.quantity, 0));
  readonly discountedSubtotal = computed(() => this.lines().reduce((sum, l) => sum + discountedUnitPrice(l.product) * l.quantity, 0));
  readonly isEmpty = computed(() => this.lines().length === 0);

  quantityOf(productId: number): number {
    return this.lines().find(l => l.product.id === productId)?.quantity ?? 0;
  }

  addQuantity(product: Product, quantity: number): void {
    if (quantity <= 0) return;
    this.lines.update(lines => {
      const existing = lines.find(l => l.product.id === product.id);
      if (existing) {
        return lines.map(l => l.product.id === product.id ? { ...l, quantity: l.quantity + quantity } : l);
      }
      return [...lines, { product, quantity }];
    });
  }

  decrease(product: Product): void {
    this.lines.update(lines =>
      lines
        .map(l => l.product.id === product.id ? { ...l, quantity: l.quantity - 1 } : l)
        .filter(l => l.quantity > 0)
    );
  }

  remove(productId: number): void {
    this.lines.update(lines => lines.filter(l => l.product.id !== productId));
  }

  clear(): void {
    this.lines.set([]);
  }
}
