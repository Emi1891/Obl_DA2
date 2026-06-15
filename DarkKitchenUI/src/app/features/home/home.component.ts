import { Component, OnInit, inject } from '@angular/core';
import { forkJoin, Observable } from 'rxjs';
import { map } from 'rxjs/operators';
import { AuthService } from '../../core/services/auth.service';
import { DashboardService } from '../../core/services/dashboard.service';
import { StatsGridComponent } from './stats-grid/stats-grid.component';
import { Stat } from './stat-card/stat-card.component';

const ICONS = {
  calendar: 'M17 12h-5v5h5v-5zM16 1v2H8V1H6v2H5c-1.11 0-1.99.9-1.99 2L3 19c0 1.1.89 2 2 2h14c1.1 0 2-.9 2-2V5c0-1.1-.9-2-2-2h-1V1h-2zm3 18H5V8h14v11z',
  truck:    'M20 8h-3V4H3c-1.1 0-2 .9-2 2v11h2c0 1.66 1.34 3 3 3s3-1.34 3-3h6c0 1.66 1.34 3 3 3s3-1.34 3-3h2v-5l-3-4zM6 18.5c-.83 0-1.5-.67-1.5-1.5s.67-1.5 1.5-1.5 1.5.67 1.5 1.5-.67 1.5-1.5 1.5zm13.5-9l1.96 2.5H17V9.5h2.5zm-1.5 9c-.83 0-1.5-.67-1.5-1.5s.67-1.5 1.5-1.5 1.5.67 1.5 1.5-.67 1.5-1.5 1.5z',
  list:     'M3 13h2v-2H3v2zm0 4h2v-2H3v2zm0-8h2V7H3v2zm4 4h14v-2H7v2zm0 4h14v-2H7v2zM7 7v2h14V7H7z',
  bag:      'M18 6h-2c0-2.21-1.79-4-4-4S8 3.79 8 6H6c-1.1 0-2 .9-2 2v12c0 1.1.9 2 2 2h12c1.1 0 2-.9 2-2V8c0-1.1-.9-2-2-2zm-6-2c1.1 0 2 .9 2 2h-4c0-1.1.9-2 2-2z',
  users:    'M16 11c1.66 0 2.99-1.34 2.99-3S17.66 5 16 5c-1.66 0-3 1.34-3 3s1.34 3 3 3zm-8 0c1.66 0 2.99-1.34 2.99-3S9.66 5 8 5C6.34 5 5 6.34 5 8s1.34 3 3 3zm0 2c-2.33 0-7 1.17-7 3.5V19h14v-2.5c0-2.33-4.67-3.5-7-3.5zm8 0c-.29 0-.62.02-.97.05 1.16.84 1.97 1.97 1.97 3.45V19h6v-2.5c0-2.33-4.67-3.5-7-3.5z',
  revenue:  'M11.8 10.9c-2.27-.59-3-1.2-3-2.15 0-1.09 1.01-1.85 2.7-1.85 1.78 0 2.44.85 2.5 2.1h2.21c-.07-1.72-1.12-3.3-3.21-3.81V3h-3v2.16c-1.94.42-3.5 1.68-3.5 3.61 0 2.31 1.91 3.46 4.7 4.13 2.5.6 3 1.48 3 2.41 0 .69-.49 1.79-2.7 1.79-2.06 0-2.87-.92-2.98-2.1h-2.2c.12 2.19 1.76 3.42 3.68 3.83V21h3v-2.15c1.95-.37 3.5-1.5 3.5-3.55 0-2.84-2.43-3.81-4.7-4.4z',
  tag:      'M21.41 11.58l-9-9C12.05 2.22 11.55 2 11 2H4c-1.1 0-2 .9-2 2v7c0 .55.22 1.05.59 1.42l9 9c.36.36.86.58 1.41.58.55 0 1.05-.22 1.41-.59l7-7c.37-.36.59-.86.59-1.41 0-.55-.23-1.06-.59-1.42zM5.5 7C4.67 7 4 6.33 4 5.5S4.67 4 5.5 4 7 4.67 7 5.5 6.33 7 5.5 7z',
};

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
          icon: ICONS.bag,
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
          icon: ICONS.tag,
          description: 'Promotions currently live and visible to customers.'
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
