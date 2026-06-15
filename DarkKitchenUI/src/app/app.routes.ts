import { Routes } from '@angular/router';
import { authGuard } from './core/guards/auth.guard';
import { permissionGuard } from './core/guards/permission.guard';

export const routes: Routes = [
  { path: '', redirectTo: 'login', pathMatch: 'full' },
  {
    path: 'login',
    loadComponent: () =>
      import('./features/auth/login/login.component').then(m => m.LoginComponent)
  },
  {
    path: 'register',
    loadComponent: () =>
      import('./features/auth/register/register.component').then(m => m.RegisterComponent)
  },
  {
    path: 'home',
    canActivate: [authGuard],
    loadComponent: () =>
      import('./features/home/home.component').then(m => m.HomeComponent)
  },
  {
    path: 'products',
    canActivate: [authGuard, permissionGuard('GetProducts')],
    loadComponent: () =>
      import('./features/products/list/products.component').then(m => m.ProductsComponent)
  },
  {
    path: 'products/new',
    canActivate: [authGuard, permissionGuard('GetProducts')],
    loadComponent: () =>
      import('./features/products/form/product-form.component').then(m => m.ProductFormComponent)
  },
  {
    path: 'products/import',
    canActivate: [authGuard, permissionGuard('GetProducts')],
    loadComponent: () =>
      import('./features/products/importer/product-importer.component').then(m => m.ProductImporterComponent)
  },
  {
    path: 'products/most-requested',
    canActivate: [authGuard, permissionGuard('GetProducts')],
    loadComponent: () =>
      import('./features/products/most-requested/most-requested.component').then(m => m.MostRequestedComponent)
  },
  {
    path: 'products/:id/edit',
    canActivate: [authGuard, permissionGuard('GetProducts')],
    loadComponent: () =>
      import('./features/products/form/product-form.component').then(m => m.ProductFormComponent)
  },
  {
    path: 'sales-report',
    canActivate: [authGuard, permissionGuard('GetSalesReport')],
    loadComponent: () =>
      import('./features/sales-report/sales-report.component').then(m => m.SalesReportComponent)
  },
  {
    path: 'cart',
    canActivate: [authGuard, permissionGuard('PlaceOrder')],
    loadComponent: () =>
      import('./features/cart/cart.component').then(m => m.CartComponent)
  },
  {
    path: 'orders',
    canActivate: [authGuard, permissionGuard(['GetMyOrders', 'GetOrdersByStatus', 'GetOrderDetails'])],
    loadComponent: () =>
      import('./features/orders/orders.component').then(m => m.OrdersComponent)
  },
  {
    path: 'audit',
    canActivate: [authGuard, permissionGuard('GetAuditRecords')],
    loadComponent: () =>
      import('./features/audit/audit.component').then(m => m.AuditComponent)
  },
  {
    path: 'users',
    canActivate: [authGuard, permissionGuard('GetUsers')],
    loadComponent: () =>
      import('./features/users/list/users.component').then(m => m.UsersComponent)
  },
  {
    path: 'users/new',
    canActivate: [authGuard, permissionGuard('GetUsers')],
    loadComponent: () =>
      import('./features/users/form/user-form.component').then(m => m.UserFormComponent)
  },
  {
    path: 'users/:email/edit',
    canActivate: [authGuard, permissionGuard('GetUsers')],
    loadComponent: () =>
      import('./features/users/form/user-form.component').then(m => m.UserFormComponent)
  },
  {
    path: 'promotions',
    canActivate: [authGuard],
    loadComponent: () =>
      import('./features/promotions/list/promotions.component').then(m => m.PromotionsComponent)
  },
  {
    path: 'promotions/new',
    canActivate: [authGuard],
    loadComponent: () =>
      import('./features/promotions/form/promotion-form.component').then(m => m.PromotionFormComponent)
  },
  {
    path: 'promotions/:id/edit',
    canActivate: [authGuard],
    loadComponent: () =>
      import('./features/promotions/form/promotion-form.component').then(m => m.PromotionFormComponent)
  },
  {
    path: 'promotions/:id/products',
    canActivate: [authGuard],
    loadComponent: () =>
      import('./features/promotions/products/promotion-products.component').then(m => m.PromotionProductsComponent)
  },
  {
    path: 'shipping-types',
    canActivate: [authGuard, permissionGuard('GetShippingTypes')],
    loadComponent: () =>
      import('./features/shipping-types/list/shipping-types.component').then(m => m.ShippingTypesComponent)
  },
  {
    path: 'shipping-types/new',
    canActivate: [authGuard, permissionGuard('CreateShippingType')],
    loadComponent: () =>
      import('./features/shipping-types/form/shipping-type-form.component').then(m => m.ShippingTypeFormComponent)
  },
  {
    path: 'shipping-types/:id/edit',
    canActivate: [authGuard, permissionGuard('UpdateShippingType')],
    loadComponent: () =>
      import('./features/shipping-types/form/shipping-type-form.component').then(m => m.ShippingTypeFormComponent)
  }
];
