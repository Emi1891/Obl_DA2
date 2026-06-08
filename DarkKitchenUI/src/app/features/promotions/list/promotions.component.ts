import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, ReactiveFormsModule } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { AuthService } from '../../../core/services/auth.service';
import { PromotionService } from '../../../core/services/promotion.service';
import { Promotion, PromotionFilters } from '../../../core/models/promotion.model';

@Component({
  selector: 'app-promotions',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, RouterLink],
  templateUrl: './promotions.component.html',
  styleUrl: './promotions.component.css'
})
export class PromotionsComponent implements OnInit {
  private readonly fb = inject(FormBuilder);
  private readonly auth = inject(AuthService);
  private readonly promotionService = inject(PromotionService);
  private readonly router = inject(Router);

  promotions: Promotion[] = [];
  isLoading = false;
  errorMessage = '';

  filtersForm = this.fb.group({
    date: [''],
    productLine: [''],
    productName: ['']
  });

  private readonly perms = new Set(this.auth.getPermissions());
  readonly canView = this.perms.has('GetCurrentPromotions');
  readonly canCreate = this.perms.has('CreatePromotion');
  readonly canEdit = this.perms.has('UpdatePromotion');
  readonly canEditProducts = this.perms.has('UpdatePromotionProducts');

  ngOnInit(): void {
    if (this.auth.isTokenExpired()) {
      this.auth.logout(true);
      return;
    }
    if (this.canView) this.load();
  }

  load(): void {
    const raw = this.filtersForm.value;
    const filters: PromotionFilters = {
      date: raw.date?.trim() || undefined,
      productLine: raw.productLine?.trim() || undefined,
      productName: raw.productName?.trim() || undefined
    };

    this.isLoading = true;
    this.errorMessage = '';
    this.promotionService.getAll(filters).subscribe({
      next: list => {
        this.promotions = list;
        this.isLoading = false;
      },
      error: () => {
        this.errorMessage = 'Could not load promotions.';
        this.isLoading = false;
      }
    });
  }

  clearFilters(): void {
    this.filtersForm.reset({ date: '', productLine: '', productName: '' });
    this.load();
  }

  edit(promotion: Promotion): void {
    this.router.navigate(['/promotions', promotion.id, 'edit'], { state: { promotion } });
  }

  manageProducts(promotion: Promotion): void {
    this.router.navigate(['/promotions', promotion.id, 'products'], { state: { promotion } });
  }

  trackById(_idx: number, p: Promotion) {
    return p.id;
  }
}
