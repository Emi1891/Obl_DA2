import { Component, OnInit, inject } from '@angular/core';
import { RouterLink } from '@angular/router';
import { Router } from '@angular/router';
import { AuthService } from '../../../core/services/auth.service';
import { ShippingTypeService } from '../../../core/services/shipping-type.service';
import { ShippingType } from '../../../core/models/order.model';

@Component({
  selector: 'app-shipping-types',
  standalone: true,
  imports: [RouterLink],
  templateUrl: './shipping-types.component.html',
  styleUrl: './shipping-types.component.css'
})
export class ShippingTypesComponent implements OnInit {
  private readonly auth = inject(AuthService);
  private readonly shippingTypeService = inject(ShippingTypeService);
  private readonly router = inject(Router);

  readonly canCreate = this.auth.hasPermission('CreateShippingType');
  readonly canEdit = this.auth.hasPermission('UpdateShippingType');

  list: ShippingType[] = [];
  isLoading = false;
  errorMessage = '';

  ngOnInit(): void {
    this.load();
  }

  load(): void {
    this.isLoading = true;
    this.shippingTypeService.getShippingTypes().subscribe({
      next: types => {
        this.list = types;
        this.isLoading = false;
      },
      error: () => {
        this.errorMessage = 'Could not load shipping types.';
        this.isLoading = false;
      }
    });
  }

  edit(t: ShippingType): void {
    this.router.navigate(['/shipping-types', t.id, 'edit']);
  }
}
