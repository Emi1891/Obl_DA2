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
    path: 'audit',
    loadComponent: () =>
      import('./features/audit/audit.component').then(m => m.AuditComponent)
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
  },
  {
    path: 'promotions',
    loadComponent: () =>
      import('./features/promotions/list/promotions.component').then(m => m.PromotionsComponent)
  },
  {
    path: 'promotions/new',
    loadComponent: () =>
      import('./features/promotions/form/promotion-form.component').then(m => m.PromotionFormComponent)
  },
  {
    path: 'promotions/:id/edit',
    loadComponent: () =>
      import('./features/promotions/form/promotion-form.component').then(m => m.PromotionFormComponent)
  },
  {
    path: 'promotions/:id/products',
    loadComponent: () =>
      import('./features/promotions/products/promotion-products.component').then(m => m.PromotionProductsComponent)
  }
];
