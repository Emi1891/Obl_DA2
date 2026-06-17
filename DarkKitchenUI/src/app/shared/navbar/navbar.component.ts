import { Component, inject } from '@angular/core';
import { RouterLink, RouterLinkActive } from '@angular/router';
import { AuthService } from '../../core/services/auth.service';
import { ICONS } from '../utils/icons';

interface NavItem {
  label: string;
  route: string;
  icon: string;
  permission?: string;
  anyPermission?: string[];
}

@Component({
  selector: 'app-navbar',
  standalone: true,
  imports: [RouterLink, RouterLinkActive],
  templateUrl: './navbar.component.html',
  styleUrl: './navbar.component.css'
})
export class NavbarComponent {
  private readonly authService = inject(AuthService);
  isExpanded = false;

  private readonly allNavItems: NavItem[] = [
    {
      label: 'Home',
      route: '/home',
      icon: ICONS.home
    },
    {
      label: 'Orders',
      route: '/orders',
      icon: ICONS.list,
      anyPermission: ['GetMyOrders', 'GetOrdersByStatus', 'GetOrderDetails']
    },
    {
      label: 'Cart',
      route: '/cart',
      icon: ICONS.cart,
      permission: 'PlaceOrder'
    },
    {
      label: 'Users',
      route: '/users',
      icon: ICONS.users,
      permission: 'GetUsers'
    },
    {
      label: 'Products',
      route: '/products',
      icon: ICONS.products,
      permission: 'GetProducts'
    },
    {
      label: 'Promotions',
      route: '/promotions',
      icon: ICONS.promotions,
      permission: 'GetCurrentPromotions'
    },
    {
      label: 'Shipping Types',
      route: '/shipping-types',
      icon: ICONS.truck,
      permission: 'UpdateShippingType'
    },
    {
      label: 'Audit',
      route: '/audit',
      icon: ICONS.audit,
      permission: 'GetAuditRecords'
    },
    {
      label: 'Sales Report',
      route: '/sales-report',
      icon: ICONS.salesReport,
      permission: 'GetSalesReport'
    },
  ];

  readonly navItems: NavItem[] = this.filterByPermissions();

  private filterByPermissions(): NavItem[] {
    return this.allNavItems.filter(item => {
      if (item.permission && !this.authService.hasPermission(item.permission)) return false;
      if (item.anyPermission && !this.authService.hasAnyPermission(item.anyPermission)) return false;
      return true;
    });
  }

  toggle() {
    this.isExpanded = !this.isExpanded;
  }

  logout() {
    this.authService.logout();
  }
}
