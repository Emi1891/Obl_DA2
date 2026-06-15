import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, ReactiveFormsModule } from '@angular/forms';
import { AuthService } from '../../core/services/auth.service';
import { OrderService } from '../../core/services/order.service';
import { OrderFilters, OrderResponse } from '../../core/models/order.model';

@Component({
  selector: 'app-orders',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './orders.component.html',
  styleUrl: './orders.component.css'
})
export class OrdersComponent implements OnInit {
  private readonly fb = inject(FormBuilder);
  private readonly auth = inject(AuthService);
  private readonly orderService = inject(OrderService);

  orders: OrderResponse[] = [];
  isLoading = false;
  errorMessage = '';
  actingOrderId: number | null = null;

  readonly statusOptions = [
    'Pending', 'Prepared', 'Canceled', 'OnItsWay', 'Delivered', 'NotDelivered', 'Delayed'
  ];

  private readonly statusPermission: Record<string, string> = {
    Prepared: 'SetOrderStatusToPrepared',
    Canceled: 'SetOrderStatusToCanceled',
    OnItsWay: 'SetOrderStatusToOnItsWay',
    Delivered: 'SetOrderStatusToDelivered',
    NotDelivered: 'SetOrderStatusToNotDelivered',
    Delayed: 'SetOrderStatusToDelayed'
  };

  private readonly validTransitions: Record<string, string[]> = {
    Pending: ['Prepared', 'Canceled', 'Delayed'],
    Prepared: ['OnItsWay'],
    OnItsWay: ['Delivered', 'NotDelivered'],
    Canceled: [],
    Delivered: [],
    NotDelivered: [],
    Delayed: ['Prepared', 'Canceled']
  };

  private readonly defaultDateFrom = '2000-01-01T00:00:00';

  private readonly perms = new Set(this.auth.getPermissions());
  readonly canViewAll = this.perms.has('GetOrdersByStatus');
  readonly canViewMine = this.perms.has('GetMyOrders');
  readonly canViewDetails = this.perms.has('GetOrderDetails');
  readonly canList = this.canViewAll || this.canViewMine;

  filtersForm = this.fb.group({
    status: [''],
    dateFrom: [''],
    dateTo: [''],
    address: ['']
  });

  lookupForm = this.fb.group({
    orderId: [null as number | null]
  });

  ngOnInit(): void {
    if (this.canList) this.load();
  }

  load(): void {
    const raw = this.filtersForm.value;
    const dateFrom = raw.dateFrom ? `${raw.dateFrom}T00:00:00` : undefined;
    const dateTo = raw.dateTo ? `${raw.dateTo}T23:59:59` : undefined;
    const status = raw.status || undefined;

    this.isLoading = true;
    this.errorMessage = '';

    const request = this.canViewAll
      ? this.orderService.getOrdersByStatus({
          dateFrom: dateFrom ?? this.defaultDateFrom,
          dateTo: dateTo ?? this.wideDateTo(),
          status,
          address: raw.address?.trim() || undefined
        })
      : this.orderService.getClientOrders({ dateFrom, dateTo, status });

    request.subscribe({
      next: list => {
        this.orders = list;
        this.isLoading = false;
      },
      error: () => {
        this.errorMessage = 'Could not load orders.';
        this.isLoading = false;
      }
    });
  }

  clearFilters(): void {
    this.filtersForm.reset({ status: '', dateFrom: '', dateTo: '', address: '' });
    this.load();
  }

  lookupOrder(): void {
    const id = this.lookupForm.value.orderId;
    if (!id) return;

    this.isLoading = true;
    this.errorMessage = '';
    this.orderService.getOrderById(id).subscribe({
      next: order => {
        this.orders = [order];
        this.isLoading = false;
      },
      error: err => {
        this.orders = [];
        this.errorMessage = err?.status === 404 ? `Order #${id} not found.` : 'Could not load order.';
        this.isLoading = false;
      }
    });
  }

  availableActions(order: OrderResponse): string[] {
    return (this.validTransitions[order.status] ?? []).filter(s => this.perms.has(this.statusPermission[s]));
  }

  updateStatus(order: OrderResponse, status: string): void {
    if (this.actingOrderId !== null) return;
    this.actingOrderId = order.id;
    this.errorMessage = '';
    this.orderService.updateOrderStatus(order.id, status).subscribe({
      next: () => {
        order.status = status;
        this.actingOrderId = null;
      },
      error: () => {
        this.errorMessage = `Could not update order #${order.id}.`;
        this.actingOrderId = null;
      }
    });
  }

  humanize(status: string): string {
    return status.replace(/([A-Z])/g, ' $1').trim();
  }

  statusClass(status: string): string {
    return status.toLowerCase();
  }

  trackById(_idx: number, order: OrderResponse): number {
    return order.id;
  }

  private wideDateTo(): string {
    return new Date(Date.now() + 365 * 24 * 60 * 60 * 1000).toISOString();
  }
}
