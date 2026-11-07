import { Component, inject, signal, OnInit } from '@angular/core';
import { DatePipe, DecimalPipe } from '@angular/common';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { Observable } from 'rxjs';
import { AuthService } from '../../core/auth/auth.service';
import { OrderService } from './order.service';
import { Order } from './order.model';

@Component({
  selector: 'app-order-detail',
  imports: [RouterLink, DatePipe, DecimalPipe],
  template: `
    <a routerLink="/orders" class="back">← Wróć do zamówień</a>

    @if (error()) {
      <div class="error">{{ error() }}</div>
    }

    @if (order(); as o) {
      <header class="head">
        <div>
          <h1>{{ o.orderNumber }}</h1>
          <span class="badge" [class]="o.status">{{ o.status }}</span>
        </div>
        <div class="actions">
          @if (o.status === 'Draft' && canSell()) {
            <button (click)="run(orderService.confirm(o.id), 'Nie udało się potwierdzić zamówienia.')">Potwierdź</button>
          }
          @if (o.status === 'StockReserved' && canWarehouse()) {
            <button class="success" (click)="run(orderService.fulfill(o.id), 'Nie udało się zrealizować zamówienia.')">Zrealizuj</button>
          }
          @if ((o.status === 'Draft' || o.status === 'StockReserved') && canSell()) {
            <button class="danger" (click)="run(orderService.cancel(o.id), 'Nie udało się anulować zamówienia.')">Anuluj</button>
          }
        </div>
      </header>

      <dl class="info">
        <dt>Klient</dt><dd>{{ o.customerName }}</dd>
        <dt>Utworzono</dt><dd>{{ o.createdAt | date: 'dd.MM.yyyy HH:mm' }}</dd>
        <dt>Potwierdzono</dt>
        <dd>{{ o.confirmedAt ? (o.confirmedAt | date: 'dd.MM.yyyy HH:mm') : '-' }}</dd>
      </dl>

      <table>
        <thead>
          <tr><th>Produkt</th><th>Ilość</th><th>Cena jedn.</th><th>Wartość</th></tr>
        </thead>
        <tbody>
          @for (i of o.items; track i.productId) {
            <tr>
              <td>{{ i.productName }}</td>
              <td>{{ i.quantity }}</td>
              <td>{{ i.unitPrice | number: '1.2-2' }}</td>
              <td>{{ i.lineTotal | number: '1.2-2' }}</td>
            </tr>
          }
        </tbody>
        <tfoot>
          <tr>
            <td colspan="3"><strong>Razem</strong></td>
            <td><strong>{{ o.totalValue | number: '1.2-2' }}</strong></td>
          </tr>
        </tfoot>
      </table>
    } @else if (!error()) {
      <p>Ładowanie...</p>
    }
  `,
  styles: `
    :host { display: block; max-width: 800px; margin: 24px auto; padding: 0 16px; }
    .back { color: #6b7280; text-decoration: none; }
    .head { display: flex; justify-content: space-between; align-items: center; margin-top: 12px; }
    .head h1 { display: inline; margin-right: 12px; }
    .actions { display: flex; gap: 8px; }
    .info { display: grid; grid-template-columns: 140px 1fr; gap: 6px 12px; margin: 16px 0; }
    dt { color: #6b7280; }
    dd { margin: 0; }
    button { padding: 8px 14px; border: none; border-radius: 6px; background: #2563eb; color: #fff; cursor: pointer; }
    button.danger { background: #dc2626; }
    button.success { background: #16a34a; }
    table { width: 100%; border-collapse: collapse; }
    th, td { text-align: left; padding: 10px; border-bottom: 1px solid #e5e7eb; }
    tfoot td { border-bottom: none; }
    .badge { padding: 2px 8px; border-radius: 999px; font-size: 0.8rem; background: #e5e7eb; }
    .badge.StockReserved { background: #fef3c7; }
    .badge.Fulfilled { background: #dcfce7; }
    .badge.Cancelled { background: #fee2e2; }
    .error { padding: 10px; background: #fee2e2; color: #b91c1c; border-radius: 6px; margin: 12px 0; }
  `,
})
export class OrderDetail implements OnInit {
  orderService = inject(OrderService);
  private route = inject(ActivatedRoute);
  auth = inject(AuthService);

  order = signal<Order | null>(null);
  error = signal<string | null>(null);

  canSell(): boolean {
    const role = this.auth.role();
    return role === 'Handlowiec' || role === 'Admin';
  }

  canWarehouse(): boolean {
    const role = this.auth.role();
    return role === 'Magazynier' || role === 'Admin';
  }

  ngOnInit(): void {
    const id = Number(this.route.snapshot.paramMap.get('id'));
    this.orderService.getById(id).subscribe({
      next: (data) => this.order.set(data),
      error: () => this.error.set('Nie znaleziono zamówienia.'),
    });
  }

  run(request: Observable<Order>, fallback: string): void {
    this.error.set(null);
    request.subscribe({
      next: (data) => this.order.set(data),
      error: (err) => this.error.set(err.error?.error ?? fallback),
    });
  }
}