import { Routes } from '@angular/router';
import { authGuard } from './core/auth/auth.guard';

export const routes: Routes = [
  { path: 'login', loadComponent: () => import('./features/auth/login/login').then(m => m.Login) },
  { path: 'register', loadComponent: () => import('./features/auth/register/register').then(m => m.Register) },
  {
    path: 'products',
    canActivate: [authGuard],
    loadComponent: () => import('./features/products/products').then(m => m.Products),
  },
  {
    path: 'customers',
    canActivate: [authGuard],
    loadComponent: () => import('./features/customers/customers').then(m => m.Customers),
  },
  {
    path: 'warehouse',
    canActivate: [authGuard],
    loadComponent: () => import('./features/warehouse/warehouse').then(m => m.Warehouse),
  },
  {
    path: 'orders',
    canActivate: [authGuard],
    loadComponent: () => import('./features/orders/orders').then(m => m.Orders),
  },
  {
    path: 'invoices',
    canActivate: [authGuard],
    loadComponent: () => import('./features/invoices/invoices').then(m => m.Invoices),
  },
  { path: '', redirectTo: 'orders', pathMatch: 'full' },
  { path: '**', redirectTo: 'orders' },
];