import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, Router } from '@angular/router';
import { AuthService } from '../../../core/services/auth.service';
import { PromotionService } from '../../../core/services/promotion.service';
import { ProductService } from '../../../core/services/product.service';
import { Promotion } from '../../../core/models/promotion.model';
import { Product } from '../../../core/models/product.model';

@Component({
  selector: 'app-promotion-products',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './promotion-products.component.html',
  styleUrl: './promotion-products.component.css'
})
export class PromotionProductsComponent implements OnInit {
  private readonly auth = inject(AuthService);
  private readonly promotionService = inject(PromotionService);
  private readonly productService = inject(ProductService);
  private readonly router = inject(Router);
  private readonly route = inject(ActivatedRoute);

  promotionId: number | null = null;
  promotion: Promotion | null = null;
  products: Product[] = [];
  selectedIds = new Set<number>();

  isLoading = false;
  isSubmitting = false;
  errorMessage = '';

  ngOnInit(): void {
    if (this.auth.isTokenExpired()) {
      this.auth.logout(true);
      return;
    }

    const idParam = this.route.snapshot.paramMap.get('id');
    if (!idParam) {
      this.router.navigate(['/promotions']);
      return;
    }

    this.promotionId = Number(idParam);
    const navState = history.state as { promotion?: Promotion };
    if (navState?.promotion && navState.promotion.id === this.promotionId) {
      this.promotion = navState.promotion;
    }

    this.loadProducts();
  }

  private loadProducts(): void {
    this.isLoading = true;
    this.errorMessage = '';
    this.productService.getProducts().subscribe({
      next: list => {
        this.products = list;
        if (this.promotion) {
          const codes = new Set(this.promotion.productCodes);
          for (const p of list) {
            if (codes.has(p.code)) this.selectedIds.add(p.id);
          }
        }
        this.isLoading = false;
      },
      error: () => {
        this.errorMessage = 'Could not load products.';
        this.isLoading = false;
      }
    });
  }

  toggle(productId: number): void {
    if (this.selectedIds.has(productId)) {
      this.selectedIds.delete(productId);
    } else {
      this.selectedIds.add(productId);
    }
  }

  isSelected(productId: number): boolean {
    return this.selectedIds.has(productId);
  }

  save(): void {
    if (this.promotionId === null) return;
    this.isSubmitting = true;
    this.errorMessage = '';
    this.promotionService.updateProducts(this.promotionId, [...this.selectedIds]).subscribe({
      next: () => this.router.navigate(['/promotions']),
      error: err => {
        const e = err as { error?: { message?: string } | string };
        const msg = typeof e.error === 'string'
          ? e.error.replace(/^[^:]+:\s*/, '')
          : e.error?.message?.replace(/^[^:]+:\s*/, '') ?? null;
        this.errorMessage = msg ?? 'Could not update promotion products.';
        this.isSubmitting = false;
      }
    });
  }

  cancel(): void {
    this.router.navigate(['/promotions']);
  }

  trackById(_idx: number, p: Product) {
    return p.id;
  }
}
