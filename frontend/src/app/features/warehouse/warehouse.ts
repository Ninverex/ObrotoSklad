import { Component, inject, signal, OnInit } from '@angular/core';
import { DatePipe } from '@angular/common';
import { RouterLink } from '@angular/router';
import { AuthService } from '../../core/auth/auth.service';
import { StockService } from './stock.service';
import { StockItem, StockMovement } from './stock.model';

@Component({
  selector: 'app-warehouse',
  imports: [RouterLink, DatePipe],
  template: `
    <header class="top">
      <h1>Magazyn</h1>
      <div>
        <a routerLink="/products">Produkty</a>
        <a routerLink="/customers">Klienci</a>
        <span>Rola: {{ auth.role() }}</span>
        <button class="secondary" (click)="auth.logout()">Wyloguj</button>
      </div>
    </header>

    @if (error()) {
      <div class="error">{{ error() }}</div>
    }

    <table>
      <thead>
        <tr>
          <th>SKU</th><th>Produkt</th><th>Na stanie</th><th>Zarezerwowane</th><th>Dostępne</th>
          <th>Przyjmij towar</th><th></th>
        </tr>
      </thead>
      <tbody>
        @for (s of items(); track s.productId) {
          <tr>
            <td>{{ s.productSku }}</td>
            <td>{{ s.productName }}</td>
            <td>{{ s.quantityOnHand }}</td>
            <td>{{ s.quantityReserved }}</td>
            <td><strong>{{ s.quantityAvailable }}</strong></td>
            <td>
              <input #qty type="number" min="1" placeholder="Ilość" />
              <button (click)="receive(s.productId, +qty.value); qty.value = ''">Przyjmij</button>
            </td>
            <td><button class="secondary" (click)="showHistory(s)">Historia</button></td>
          </tr>
        } @empty {
          <tr><td colspan="7">Brak pozycji magazynowych.</td></tr>
        }
      </tbody>
    </table>

    @if (selected()) {
      <h2>Historia ruchów: {{ selected()!.productName }}</h2>
      <table>
        <thead>
          <tr><th>Data</th><th>Typ</th><th>Ilość</th><th>Zamówienie</th></tr>
        </thead>
        <tbody>
          @for (m of history(); track m.id) {
            <tr>
              <td>{{ m.createdAt | date: 'dd.MM.yyyy HH:mm' }}</td>
              <td>{{ m.type }}</td>
              <td>{{ m.quantity }}</td>
              <td>{{ m.orderId ?? '-' }}</td>
            </tr>
          } @empty {
            <tr><td colspan="4">Brak ruchów.</td></tr>
          }
        </tbody>
      </table>
    }
  `,
  styles: `
    :host { display: block; max-width: 1100px; margin: 24px auto; padding: 0 16px; }
    .top { display: flex; justify-content: space-between; align-items: center; }
    .top span, .top a { margin-right: 12px; color: #6b7280; }
    input { width: 80px; padding: 6px 8px; border: 1px solid #d1d5db; border-radius: 6px; margin-right: 6px; }
    button { padding: 6px 12px; border: none; border-radius: 6px; background: #2563eb; color: #fff; cursor: pointer; }
    button.secondary { background: #6b7280; }
    table { width: 100%; border-collapse: collapse; margin-bottom: 24px; }
    th, td { text-align: left; padding: 10px; border-bottom: 1px solid #e5e7eb; }
    .error { padding: 10px; background: #fee2e2; color: #b91c1c; border-radius: 6px; margin: 12px 0; }
  `,
})
export class Warehouse implements OnInit {
  private service = inject(StockService);
  auth = inject(AuthService);

  items = signal<StockItem[]>([]);
  history = signal<StockMovement[]>([]);
  selected = signal<StockItem | null>(null);
  error = signal<string | null>(null);

  ngOnInit(): void {
    this.load();
  }

  load(): void {
    this.service.getAll().subscribe({
      next: (data) => this.items.set(data),
      error: () => this.error.set('Nie udało się pobrać stanów magazynowych.'),
    });
  }

  receive(productId: number, quantity: number): void {
    if (!quantity || quantity <= 0) {
      this.error.set('Podaj ilość większą od zera.');
      return;
    }
    this.error.set(null);

    this.service.receive({ productId, quantity }).subscribe({
      next: () => {
        this.load();
        if (this.selected()?.productId === productId) {
          this.loadHistory(productId);
        }
      },
      error: (err) => this.error.set(err.error?.error ?? 'Nie udało się przyjąć towaru.'),
    });
  }

  showHistory(item: StockItem): void {
    this.selected.set(item);
    this.loadHistory(item.productId);
  }

  private loadHistory(productId: number): void {
    this.service.getHistory(productId).subscribe({
      next: (data) => this.history.set(data),
      error: () => this.error.set('Nie udało się pobrać historii.'),
    });
  }
}