import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { ProductService } from '../../../core/services/product.service';
import { Product } from '../../../core/models/product.model';
import { oneMonthAgoRange } from '../../../shared/utils/date';

@Component({
  selector: 'app-most-requested',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './most-requested.component.html',
  styleUrl: './most-requested.component.css'
})
export class MostRequestedComponent implements OnInit {
  private readonly fb = inject(FormBuilder);
  private readonly productService = inject(ProductService);
  private readonly router = inject(Router);

  products: Product[] = [];
  isLoading = false;
  errorMessage = '';

  form = this.fb.group({
    dateFrom: ['', Validators.required],
    dateTo: ['', Validators.required]
  });

  ngOnInit(): void {
    this.form.patchValue(oneMonthAgoRange());
    this.load();
  }

  load(): void {
    if (this.form.invalid) return;
    const { dateFrom, dateTo } = this.form.getRawValue();
    this.isLoading = true;
    this.errorMessage = '';
    this.productService.getMostRequested({
      dateFrom: `${dateFrom}T00:00:00`,
      dateTo: `${dateTo}T23:59:59`
    }).subscribe({
      next: list => {
        this.products = list;
        this.isLoading = false;
      },
      error: () => {
        this.errorMessage = 'Could not load report.';
        this.isLoading = false;
      }
    });
  }

  back(): void {
    this.router.navigate(['/products']);
  }

  rankIcon(idx: number): string {
    if (idx === 0) return '#1';
    if (idx === 1) return '#2';
    if (idx === 2) return '#3';
    return `#${idx + 1}`;
  }
}
