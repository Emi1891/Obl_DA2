import { Routes } from '@angular/router';

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
    loadComponent: () =>
      import('./features/home/home.component').then(m => m.HomeComponent)
  },
  {
    path: 'products',
    loadComponent: () =>
      import('./features/products/list/products.component').then(m => m.ProductsComponent)
  },
  {
    path: 'products/new',
    loadComponent: () =>
      import('./features/products/form/product-form.component').then(m => m.ProductFormComponent)
  },
  {
    path: 'products/import',
    loadComponent: () =>
      import('./features/products/importer/product-importer.component').then(m => m.ProductImporterComponent)
  },
  {
    path: 'products/most-requested',
    loadComponent: () =>
      import('./features/products/most-requested/most-requested.component').then(m => m.MostRequestedComponent)
  },
  {
    path: 'products/:id/edit',
    loadComponent: () =>
      import('./features/products/form/product-form.component').then(m => m.ProductFormComponent)
  },
  {
    path: 'sales-report',
    loadComponent: () =>
      import('./features/sales-report/sales-report.component').then(m => m.SalesReportComponent)
  },
  {
    path: 'cart',
    loadComponent: () =>
      import('./features/cart/cart.component').then(m => m.CartComponent)
  },
  {
    path: 'orders',
    loadComponent: () =>
      import('./features/orders/orders.component').then(m => m.OrdersComponent)
  },
  {
    path: 'users',
    loadComponent: () =>
      import('./features/users/list/users.component').then(m => m.UsersComponent)
  },
  {
    path: 'users/new',
    loadComponent: () =>
      import('./features/users/form/user-form.component').then(m => m.UserFormComponent)
  },
  {
    path: 'users/:email/edit',
    loadComponent: () =>
      import('./features/users/form/user-form.component').then(m => m.UserFormComponent)
  }
];
