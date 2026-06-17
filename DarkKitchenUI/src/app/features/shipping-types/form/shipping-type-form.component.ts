import { Component, OnInit, inject } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { ShippingTypeService } from '../../../core/services/shipping-type.service';

@Component({
  selector: 'app-shipping-type-form',
  standalone: true,
  imports: [ReactiveFormsModule],
  templateUrl: './shipping-type-form.component.html',
  styleUrl: './shipping-type-form.component.css'
})
export class ShippingTypeFormComponent implements OnInit {
  private readonly fb = inject(FormBuilder);
  private readonly route = inject(ActivatedRoute);
  private readonly router = inject(Router);
  private readonly shippingTypeService = inject(ShippingTypeService);

  isEdit = false;
  isSubmitting = false;
  errorMessage = '';
  private editId: number | null = null;

  form = this.fb.group({
    name: ['', Validators.required],
    price: [null as number | null, [Validators.required, Validators.min(0)]]
  });

  ngOnInit(): void {
    const id = this.route.snapshot.paramMap.get('id');
    if (id) {
      this.isEdit = true;
      this.editId = +id;
      this.shippingTypeService.getShippingTypes().subscribe({
        next: types => {
          const found = types.find(t => t.id === this.editId);
          if (found) {
            this.form.patchValue({ name: found.name, price: found.price });
          } else {
            this.router.navigate(['/shipping-types']);
          }
        },
        error: () => this.router.navigate(['/shipping-types'])
      });
    }
  }

  onSubmit(): void {
    if (this.form.invalid) return;
    this.isSubmitting = true;
    this.errorMessage = '';

    const { name, price } = this.form.getRawValue();
    const payload = { name: name!, price: price! };

    const request = this.isEdit
      ? this.shippingTypeService.updateShippingType(this.editId!, payload)
      : this.shippingTypeService.createShippingType(payload);

    request.subscribe({
      next: () => this.router.navigate(['/shipping-types']),
      error: () => {
        this.errorMessage = 'Could not save shipping type. Please try again.';
        this.isSubmitting = false;
      }
    });
  }

  cancel(): void {
    this.router.navigate(['/shipping-types']);
  }
}
