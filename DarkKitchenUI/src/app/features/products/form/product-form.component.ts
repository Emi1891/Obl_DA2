import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormArray, FormBuilder, FormControl, ReactiveFormsModule, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { ProductService } from '../../../core/services/product.service';
import { CreateProductRequest, Product, UpdateProductRequest } from '../../../core/models/product.model';
import { extractError } from '../../../shared/utils/error';

@Component({
  selector: 'app-product-form',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './product-form.component.html',
  styleUrl: './product-form.component.css'
})
export class ProductFormComponent implements OnInit {
  private readonly fb = inject(FormBuilder);
  private readonly productService = inject(ProductService);
  private readonly router = inject(Router);
  private readonly route = inject(ActivatedRoute);

  productId: number | null = null;
  isEdit = false;
  errorMessage = '';
  isSubmitting = false;

  form = this.fb.group({
    code: ['', Validators.required],
    name: ['', Validators.required],
    description: [''],
    productLine: [''],
    category: [''],
    price: [0, [Validators.required, Validators.min(0.01)]],
    isActive: [true],
    imageUrl: this.fb.array<FormControl<string>>([])
  });

  ngOnInit(): void {
    const idParam = this.route.snapshot.paramMap.get('id');
    if (idParam) {
      this.isEdit = true;
      this.productId = Number(idParam);
      const navState = history.state as { product?: Product };
      if (navState?.product && navState.product.id === this.productId) {
        this.patchFromProduct(navState.product);
      }
    } else {
      this.addImage();
    }
  }

  get images(): FormArray<FormControl<string>> {
    return this.form.get('imageUrl') as FormArray<FormControl<string>>;
  }

  readonly maxImages = 3;

  addImage(value = ''): void {
    if (this.images.length >= this.maxImages) return;
    this.images.push(this.fb.nonNullable.control(value));
  }

  removeImage(index: number): void {
    this.images.removeAt(index);
  }

  private patchFromProduct(p: Product): void {
    this.form.patchValue({
      code: p.code,
      name: p.name,
      description: p.description ?? '',
      productLine: p.productLine ?? '',
      category: p.category ?? '',
      price: p.price,
      isActive: p.isActive
    });
    this.images.clear();
    const urls = p.imageUrl?.length ? p.imageUrl : [''];
    for (const url of urls) this.addImage(url);
  }

  onSubmit(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }
    this.isSubmitting = true;
    this.errorMessage = '';

    const raw = this.form.getRawValue();
    const cleanedImages = raw.imageUrl.map(u => u.trim()).filter(Boolean);

    if (this.isEdit && this.productId !== null) {
      const payload: UpdateProductRequest = {
        id: this.productId,
        code: raw.code || null,
        name: raw.name || null,
        description: raw.description || null,
        productLine: raw.productLine || null,
        category: raw.category || null,
        price: raw.price ?? null,
        imageUrl: cleanedImages.length ? cleanedImages : null,
        isActive: raw.isActive
      };
      this.productService.updateProduct(payload).subscribe({
        next: () => this.router.navigate(['/products']),
        error: err => {
          this.errorMessage = extractError(err) ?? 'Could not update product.';
          this.isSubmitting = false;
        }
      });
    } else {
      const payload: CreateProductRequest = {
        code: raw.code!,
        name: raw.name!,
        description: raw.description || null,
        productLine: raw.productLine || null,
        category: raw.category || null,
        price: raw.price!,
        imageUrl: cleanedImages
      };
      this.productService.createProduct(payload).subscribe({
        next: () => this.router.navigate(['/products']),
        error: err => {
          this.errorMessage = extractError(err) ?? 'Could not create product.';
          this.isSubmitting = false;
        }
      });
    }
  }

  cancel(): void {
    this.router.navigate(['/products']);
  }
}
