import { Component, OnInit, inject } from '@angular/core';
import { forkJoin, Observable } from 'rxjs';
import { map } from 'rxjs/operators';
import { AuthService } from '../../core/services/auth.service';
import { DashboardService } from '../../core/services/dashboard.service';
import { StatsGridComponent } from './stats-grid/stats-grid.component';
import { Stat } from './stat-card/stat-card.component';
import { ICONS } from '../../shared/utils/icons';

function fmt(n: number | null, format: (v: number) => string): string {
  return n !== null ? format(n) : '—';
}

@Component({
  selector: 'app-home',
  standalone: true,
  imports: [StatsGridComponent],
  templateUrl: './home.component.html',
  styleUrl: './home.component.css'
})
export class HomeComponent implements OnInit {
  private readonly auth = inject(AuthService);
  private readonly dashboard = inject(DashboardService);

  readonly userName = this.auth.getUserName();
  stats: Stat[] = [];
  isLoading = true;

  ngOnInit(): void {
    const perms = new Set(this.auth.getPermissions());
    const calls: Observable<Stat>[] = [];

    if (perms.has('GetOrdersByStatus')) {
      calls.push(this.dashboard.getOrderCountByStatus('Pending').pipe(
        map(n => ({
          label: 'Pending Orders',
          value: fmt(n, String),
          icon: ICONS.calendar,
          description: 'Orders received and waiting to be prepared in the kitchen.'
        }))
      ));
      calls.push(this.dashboard.getOrderCountByStatus('OnItsWay').pipe(
        map(n => ({
          label: 'In Transit',
          value: fmt(n, String),
          icon: ICONS.truck,
          description: 'Orders currently on their way to customers.'
        }))
      ));
    }

    if (perms.has('GetMyOrders')) {
      calls.push(this.dashboard.getClientOrderCount().pipe(
        map(n => ({
          label: 'My Orders',
          value: fmt(n, String),
          icon: ICONS.list,
          description: 'Total orders you have placed through the platform.'
        }))
      ));
    }

    if (perms.has('GetProducts')) {
      calls.push(this.dashboard.getProductCount(!perms.has('UpdateProductStatus')).pipe(
        map(n => ({
          label: 'Products',
          value: fmt(n, String),
          icon: ICONS.products,
          description: 'Products currently listed in the catalog.'
        }))
      ));
    }

    if (perms.has('GetUsers')) {
      calls.push(this.dashboard.getUserCount().pipe(
        map(n => ({
          label: 'Registered Users',
          value: fmt(n, String),
          icon: ICONS.users,
          description: 'Total users registered across all roles in the system.'
        }))
      ));
    }

    if (perms.has('GetSalesReport')) {
      calls.push(this.dashboard.getTotalRevenue().pipe(
        map(n => ({
          label: 'Total Revenue',
          value: fmt(n, v => `$${v.toFixed(2)}`),
          icon: ICONS.revenue,
          description: 'Cumulative revenue generated from all completed orders.'
        }))
      ));
    }

    if (perms.has('GetCurrentPromotions')) {
      calls.push(this.dashboard.getPromotionCount().pipe(
        map(n => ({
          label: 'Active Promotions',
          value: fmt(n, String),
          icon: ICONS.promotions,
          description: 'Promotions currently live and visible to customers.'
        }))
      ));
    }

    if (perms.has('GetShippingTypes')) {
      calls.push(this.dashboard.getShippingTypeCount().pipe(
        map(n => ({
          label: 'Shipping Types',
          value: fmt(n, String),
          icon: ICONS.truck,
          description: 'Delivery methods available at checkout for customers.',
          link: '/shipping-types'
        }))
      ));
    }

    if (calls.length === 0) {
      this.isLoading = false;
      return;
    }

    forkJoin(calls).subscribe({
      next: results => { this.stats = results; this.isLoading = false; },
      error: () => { this.isLoading = false; }
    });
  }
}
