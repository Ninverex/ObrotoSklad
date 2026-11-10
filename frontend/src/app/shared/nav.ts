import { Component, inject } from '@angular/core';
import { RouterLink, RouterLinkActive } from '@angular/router';
import { AuthService } from '../core/auth/auth.service';

@Component({
  selector: 'app-nav',
  imports: [RouterLink, RouterLinkActive],
  template: `
    <nav>
      <a routerLink="/orders" routerLinkActive="active">Zamówienia</a>
      <a routerLink="/invoices" routerLinkActive="active">Faktury</a>
      <a routerLink="/warehouse" routerLinkActive="active">Magazyn</a>
      @if (auth.role() === 'Admin' || auth.role() === 'Magazynier') {
        <a routerLink="/products" routerLinkActive="active">Produkty</a>
      }
      @if (auth.role() === 'Admin' || auth.role() === 'Handlowiec') {
        <a routerLink="/customers" routerLinkActive="active">Klienci</a>
      }
      <span class="spacer"></span>
      <span class="user">{{ auth.role() }}</span>
      <button (click)="auth.logout()">Wyloguj</button>
    </nav>
  `,
  styles: `
    nav { display: flex; align-items: center; gap: 16px; padding: 12px 0; border-bottom: 1px solid #e5e7eb; margin-bottom: 16px; }
    a { color: #6b7280; text-decoration: none; }
    a.active { color: #2563eb; font-weight: 600; }
    .spacer { flex: 1; }
    .user { color: #6b7280; }
    button { padding: 6px 12px; border: none; border-radius: 6px; background: #6b7280; color: #fff; cursor: pointer; }
  `,
})
export class Nav {
  auth = inject(AuthService);
}