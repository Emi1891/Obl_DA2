import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { AuthService } from '../../core/services/auth.service';
import { ShippingTypeService } from '../../core/services/shipping-type.service';
import { OrderService } from '../../core/services/order.service';
import { CartService } from '../../core/services/cart.service';
import { Product } from '../../core/models/product.model';
import { CreateOrderRequest, OrderResponse, ShippingType } from '../../core/models/order.model';

@Component({
  selector: 'app-cart',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, RouterLink],
  templateUrl: './cart.component.html',
  styleUrl: './cart.component.css'
})
export class CartComponent implements OnInit {
  private readonly fb = inject(FormBuilder);
  private readonly auth = inject(AuthService);
  private readonly shippingTypeService = inject(ShippingTypeService);
  private readonly orderService = inject(OrderService);
  private readonly router = inject(Router);
  readonly cart = inject(CartService);

  shippingTypes: ShippingType[] = [];

  isLoading = false;
  isPlacingOrder = false;
  errorMessage = '';
  placedOrder: OrderResponse | null = null;

  private readonly perms = new Set(this.auth.getPermissions());
  readonly canPlaceOrder = this.perms.has('PlaceOrder');

  checkoutForm = this.fb.group({
    street: ['', Validators.required],
    doorNumber: ['', Validators.required],
    apartment: [''],
    shippingTypeId: [null as number | null, Validators.required]
  });

  ngOnInit(): void {
    if (this.auth.isTokenExpired()) {
      this.auth.logout(true);
      return;
    }
    if (this.canPlaceOrder) this.loadShippingTypes();
  }

  loadShippingTypes(): void {
    this.isLoading = true;
    this.errorMessage = '';
    this.shippingTypeService.getShippingTypes().subscribe({
      next: types => {
        this.shippingTypes = types;
        this.isLoading = false;
      },
      error: () => {
        this.errorMessage = 'Could not load shipping options.';
        this.isLoading = false;
      }
    });
  }

  add(product: Product): void {
    this.cart.add(product);
  }

  decrease(product: Product): void {
    this.cart.decrease(product);
  }

  remove(productId: number): void {
    this.cart.remove(productId);
  }

  get selectedShipping(): ShippingType | null {
    const id = this.checkoutForm.value.shippingTypeId;
    return this.shippingTypes.find(t => t.id === id) ?? null;
  }

  get estimatedTotal(): number {
    return this.cart.subtotal() + (this.selectedShipping?.price ?? 0);
  }

  placeOrder(): void {
    if (this.checkoutForm.invalid || this.cart.isEmpty() || this.isPlacingOrder) {
      this.checkoutForm.markAllAsTouched();
      return;
    }

    const raw = this.checkoutForm.getRawValue();
    const request: CreateOrderRequest = {
      shippingTypeId: raw.shippingTypeId!,
      address: {
        street: raw.street!.trim(),
        doorNumber: raw.doorNumber!.trim(),
        apartment: raw.apartment?.trim() || null
      },
      products: this.cart.items().map(line => ({
        productCode: line.product.code,
        quantity: line.quantity
      }))
    };

    this.isPlacingOrder = true;
    this.errorMessage = '';
    this.orderService.createOrder(request).subscribe({
      next: order => {
        this.placedOrder = order;
        this.cart.clear();
        this.checkoutForm.reset({ street: '', doorNumber: '', apartment: '', shippingTypeId: null });
        this.isPlacingOrder = false;
      },
      error: () => {
        this.errorMessage = 'Could not place the order. Please try again.';
        this.isPlacingOrder = false;
      }
    });
  }

  dismissConfirmation(): void {
    this.placedOrder = null;
  }

  back(): void {
    this.router.navigate(['/home']);
  }
}
