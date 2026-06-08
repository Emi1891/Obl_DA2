import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { AuthService } from '../../../core/services/auth.service';
import { PromotionService } from '../../../core/services/promotion.service';
import { Promotion, PromotionRequest } from '../../../core/models/promotion.model';

@Component({
  selector: 'app-promotion-form',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './promotion-form.component.html',
  styleUrl: './promotion-form.component.css'
})
export class PromotionFormComponent implements OnInit {
  private readonly fb = inject(FormBuilder);
  private readonly auth = inject(AuthService);
  private readonly promotionService = inject(PromotionService);
  private readonly router = inject(Router);
  private readonly route = inject(ActivatedRoute);

  promotionId: number | null = null;
  isEdit = false;
  errorMessage = '';
  isSubmitting = false;

  form = this.fb.group({
    name: ['', Validators.required],
    discountPercentage: [0, [Validators.required, Validators.min(1), Validators.max(100)]],
    dateFrom: ['', Validators.required],
    dateTo: ['', Validators.required]
  });

  ngOnInit(): void {
    if (this.auth.isTokenExpired()) {
      this.auth.logout(true);
      return;
    }

    const idParam = this.route.snapshot.paramMap.get('id');
    if (idParam) {
      this.isEdit = true;
      this.promotionId = Number(idParam);
      const navState = history.state as { promotion?: Promotion };
      if (navState?.promotion && navState.promotion.id === this.promotionId) {
        this.form.patchValue({
          name: navState.promotion.name,
          discountPercentage: navState.promotion.discountPercentage,
          dateFrom: navState.promotion.dateFrom.split('T')[0],
          dateTo: navState.promotion.dateTo.split('T')[0]
        });
      }
    }
  }

  onSubmit(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }
    this.isSubmitting = true;
    this.errorMessage = '';

    const raw = this.form.getRawValue();
    const payload: PromotionRequest = {
      name: raw.name!,
      discountPercentage: raw.discountPercentage!,
      dateFrom: new Date(raw.dateFrom!).toISOString(),
      dateTo: new Date(raw.dateTo!).toISOString()
    };

    if (this.isEdit && this.promotionId !== null) {
      this.promotionService.update(this.promotionId, payload).subscribe({
        next: () => this.router.navigate(['/promotions']),
        error: err => {
          this.errorMessage = this.extractError(err) ?? 'Could not update promotion.';
          this.isSubmitting = false;
        }
      });
    } else {
      this.promotionService.create(payload).subscribe({
        next: () => this.router.navigate(['/promotions']),
        error: err => {
          this.errorMessage = this.extractError(err) ?? 'Could not create promotion.';
          this.isSubmitting = false;
        }
      });
    }
  }

  private extractError(err: unknown): string | null {
    const e = err as { error?: { message?: string } | string };
    if (typeof e.error === 'string') return e.error.replace(/^[^:]+:\s*/, '');
    return e.error?.message?.replace(/^[^:]+:\s*/, '') ?? null;
  }

  cancel(): void {
    this.router.navigate(['/promotions']);
  }
}
