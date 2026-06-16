import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, ReactiveFormsModule } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { AuthService } from '../../../core/services/auth.service';
import { ProductService } from '../../../core/services/product.service';
import { CartService } from '../../../core/services/cart.service';
import { DigitsOnlyDirective } from '../../../shared/directives/digits-only.directive';
import { Product, ProductFilters, discountedUnitPrice } from '../../../core/models/product.model';

@Component({
  selector: 'app-products',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, RouterLink, DigitsOnlyDirective],
  templateUrl: './products.component.html',
  styleUrl: './products.component.css'
})
export class ProductsComponent implements OnInit {
  private readonly fb = inject(FormBuilder);
  private readonly auth = inject(AuthService);
  private readonly productService = inject(ProductService);
  private readonly router = inject(Router);
  readonly cart = inject(CartService);

  products: Product[] = [];
  isLoading = false;
  errorMessage = '';

  filtersForm = this.fb.group({
    name: [''],
    productLine: [''],
    categories: ['']
  });

  readonly canView = this.auth.hasPermission('GetProducts');
  readonly canCreate = this.auth.hasPermission('CreateProduct');
  readonly canEdit = this.auth.hasPermission('UpdateProduct');
  readonly canToggleStatus = this.auth.hasPermission('UpdateProductStatus');
  readonly canImport = this.auth.hasPermission('ImportProducts');
  readonly canSeeMostRequested = this.auth.hasPermission('GetMostPopularProducts');
  readonly canAddToCart = this.auth.hasPermission('PlaceOrder');

  ngOnInit(): void {
    if (this.canView) this.load();
  }

  get visibleProducts(): Product[] {
    return this.canToggleStatus ? this.products : this.products.filter(p => p.isActive);
  }

  private readonly selectedQty = new Map<number, number>();
  readonly maxQty = 99;

  qtyFor(product: Product): number {
    return this.selectedQty.get(product.id) ?? 1;
  }

  isSelecting(product: Product): boolean {
    return this.selectedQty.has(product.id);
  }

  startAdding(product: Product): void {
    this.selectedQty.set(product.id, 1);
  }

  incQty(product: Product): void {
    this.selectedQty.set(product.id, Math.min(this.maxQty, this.qtyFor(product) + 1));
  }

  decQty(product: Product): void {
    if (this.qtyFor(product) - 1 < 1) {
      this.selectedQty.delete(product.id);
    } else {
      this.selectedQty.set(product.id, this.qtyFor(product) - 1);
    }
  }

  setQty(product: Product, event: Event): void {
    const input = event.target as HTMLInputElement;
    const parsed = parseInt(input.value, 10);
    if (!Number.isFinite(parsed) || parsed < 1) {
      this.selectedQty.delete(product.id);
      return;
    }
    const qty = Math.min(parsed, this.maxQty);
    this.selectedQty.set(product.id, qty);
    input.value = String(qty);
  }

  confirm(product: Product): void {
    this.cart.addQuantity(product, this.qtyFor(product));
    this.selectedQty.delete(product.id);
  }

  load(): void {
    const raw = this.filtersForm.value;
    const categories = (raw.categories ?? '').split(',').map(s => s.trim()).filter(Boolean);
    const filters: ProductFilters = {
      name: raw.name?.trim() || undefined,
      productLine: raw.productLine?.trim() || undefined,
      categories: categories.length ? categories : undefined
    };

    this.isLoading = true;
    this.errorMessage = '';
    this.productService.getProducts(filters).subscribe({
      next: list => {
        this.products = list;
        this.isLoading = false;
      },
      error: () => {
        this.errorMessage = 'Could not load products.';
        this.isLoading = false;
      }
    });
  }

  clearFilters(): void {
    this.filtersForm.reset({ name: '', productLine: '', categories: '' });
    this.load();
  }

  toggleStatus(product: Product): void {
    if (!this.canToggleStatus) return;
    const next = !product.isActive;
    this.productService.updateStatus(product.id, next).subscribe({
      next: () => product.isActive = next,
      error: () => this.errorMessage = `Could not update status of "${product.name}".`
    });
  }

  edit(product: Product): void {
    this.router.navigate(['/products', product.id, 'edit'], { state: { product } });
  }

  private readonly imageIndex = new Map<number, number>();

  imageIdx(product: Product): number {
    return this.imageIndex.get(product.id) ?? 0;
  }

  setImage(product: Product, index: number): void {
    this.imageIndex.set(product.id, index);
  }

  prevImage(product: Product): void {
    const count = product.imageUrl?.length ?? 0;
    if (count === 0) return;
    this.imageIndex.set(product.id, (this.imageIdx(product) - 1 + count) % count);
  }

  nextImage(product: Product): void {
    const count = product.imageUrl?.length ?? 0;
    if (count === 0) return;
    this.imageIndex.set(product.id, (this.imageIdx(product) + 1) % count);
  }

  discountedPrice(product: Product): number {
    return discountedUnitPrice(product);
  }

  trackById(_idx: number, p: Product) {
    return p.id;
  }
}
