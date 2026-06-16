import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { RouterLink } from '@angular/router';
import { AuthService } from '../../core/services/auth.service';
import { ShippingTypeService } from '../../core/services/shipping-type.service';
import { OrderService } from '../../core/services/order.service';
import { CartService, CartLine } from '../../core/services/cart.service';
import { Product, discountedUnitPrice } from '../../core/models/product.model';
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
  readonly cart = inject(CartService);

  shippingTypes: ShippingType[] = [];

  isLoading = false;
  isPlacingOrder = false;
  errorMessage = '';
  placedOrder: OrderResponse | null = null;

  readonly canPlaceOrder = this.auth.hasPermission('PlaceOrder');

  checkoutForm = this.fb.group({
    street: ['', Validators.required],
    doorNumber: ['', Validators.required],
    apartment: [''],
    shippingTypeId: [null as number | null, Validators.required]
  });

  ngOnInit(): void {
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
    this.cart.addQuantity(product, 1);
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

  private readonly vatRate = 0.22;

  get estimatedVat(): number {
    return this.cart.discountedSubtotal() * this.vatRate;
  }

  get estimatedTotal(): number {
    return this.cart.discountedSubtotal() + this.estimatedVat + (this.selectedShipping?.price ?? 0);
  }

  unitPrice(product: Product): number {
    return discountedUnitPrice(product);
  }

  lineTotal(line: CartLine): number {
    return discountedUnitPrice(line.product) * line.quantity;
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
}
