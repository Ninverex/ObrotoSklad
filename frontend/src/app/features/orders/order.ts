import { Component, inject, signal, OnInit } from '@angular/core';
import { DatePipe, DecimalPipe } from '@angular/common';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { RouterLink } from '@angular/router';
import { AuthService } from '../../core/auth/auth.service';
import { CustomerService } from '../customers/customer.service';
import { ProductService } from '../products/product.service';
import { Customer } from '../customers/customer.model';
import { Product } from '../products/product.model';
import { OrderService } from './order.service';
import { Order } from './order.model';

@Component({
  selector: 'app-orders',
  imports: [ReactiveFormsModule, RouterLink, DatePipe, DecimalPipe],
  template: `
    <header class="top">
      <h1>Zamówienia</h1>
      <div>
        <a routerLink="/products">Produkty</a>
        <a routerLink="/customers">Klienci</a>
        <a routerLink="/warehouse">Magazyn</a>
        <span>Rola: {{ auth.role() }}</span>
        <button class="secondary" (click)="auth.logout()">Wyloguj</button>
      </div>
    </header>

    @if (error()) {
      <div class="error">{{ error() }}</div>
    }

    @if (canCreate()) {
      <form class="new" [formGroup]="form" (ngSubmit)="create()">
        <h2>Nowe zamówienie</h2>

        <select formControlName="customerId">
          <option [ngValue]="0" disabled>Wybierz klienta</option>
          @for (c of customers(); track c.id) {
            <option [ngValue]="c.id">{{ c.name }}</option>
          }
        </select>

        @for (line of lines.controls; track $index) {
          <div class="line" [formGroup]="line">
            <select formControlName="productId">
              <option [ngValue]="0" disabled>Wybierz produkt</option>
              @for (p of products(); track p.id) {
                <option [ngValue]="p.id">{{ p.name }} ({{ p.price | number: '1.2-2' }})</option>
              }
            </select>
            <input type="number" min="1" formControlName="quantity" />
            <button type="button" class="danger" (click)="removeLine($index)" [disabled]="lines.length === 1">
              ×
            </button>
          </div>
        }

        <div class="actions">
          <button type="button" class="secondary" (click)="addLine()">+ Dodaj pozycję</button>
          <button type="submit" [disabled]="form.invalid">Utwórz zamówienie</button>
        </div>
      </form>
    }

    <table>
      <thead>
        <tr>
          <th>Numer</th><th>Klient</th><th>Status</th><th>Utworzono</th>
          <th>Pozycje</th><th>Wartość</th><th>Akcje</th>
        </tr>
      </thead>
      <tbody>
        @for (o of orders(); track o.id) {
          <tr>
            <td>{{ o.orderNumber }}</td>
            <td>{{ o.customerName }}</td>
            <td><span class="badge" [class]="o.status">{{ o.status }}</span></td>
            <td>{{ o.createdAt | date: 'dd.MM.yyyy HH:mm' }}</td>
            <td>
              @for (i of o.items; track i.productId) {
                <div>{{ i.productName }} × {{ i.quantity }}</div>
              }
            </td>
            <td>{{ o.totalValue | number: '1.2-2' }}</td>
            <td class="row-actions">
              @if (o.status === 'Draft' && canSell()) {
                <button (click)="confirm(o.id)">Potwierdź</button>
              }
              @if (o.status === 'StockReserved' && canWarehouse()) {
                <button class="success" (click)="fulfill(o.id)">Zrealizuj</button>
              }
              @if ((o.status === 'Draft' || o.status === 'StockReserved') && canSell()) {
                <button class="danger" (click)="cancel(o.id)">Anuluj</button>
              }
            </td>
          </tr>
        } @empty {
          <tr><td colspan="7">Brak zamówień.</td></tr>
        }
      </tbody>
    </table>
  `,
  styles: `
    :host { display: block; max-width: 1100px; margin: 24px auto; padding: 0 16px; }
    .top { display: flex; justify-content: space-between; align-items: center; }
    .top span, .top a { margin-right: 12px; color: #6b7280; }
    .new { display: flex; flex-direction: column; gap: 8px; margin: 16px 0; padding: 16px;
           background: #f9fafb; border-radius: 8px; }
    .new h2 { margin: 0; font-size: 1.1rem; }
    .line { display: flex; gap: 8px; }
    .actions { display: flex; gap: 8px; }
    select, input { padding: 8px 10px; border: 1px solid #d1d5db; border-radius: 6px; }
    .line select { flex: 1; }
    .line input { width: 90px; }
    button { padding: 8px 14px; border: none; border-radius: 6px; background: #2563eb; color: #fff; cursor: pointer; }
    button:disabled { background: #93c5fd; cursor: not-allowed; }
    button.secondary { background: #6b7280; }
    button.danger { background: #dc2626; }
    button.success { background: #16a34a; }
    .row-actions { display: flex; gap: 6px; }
    table { width: 100%; border-collapse: collapse; }
    th, td { text-align: left; padding: 10px; border-bottom: 1px solid #e5e7eb; vertical-align: top; }
    .badge { padding: 2px 8px; border-radius: 999px; font-size: 0.8rem; background: #e5e7eb; }
    .badge.Draft { background: #e5e7eb; }
    .badge.StockReserved { background: #fef3c7; }
    .badge.Fulfilled { background: #dcfce7; }
    .badge.Cancelled { background: #fee2e2; }
    .error { padding: 10px; background: #fee2e2; color: #b91c1c; border-radius: 6px; margin: 12px 0; }
  `,
})
export class Orders implements OnInit {
  private fb = inject(FormBuilder);
  private orderService = inject(OrderService);
  private customerService = inject(CustomerService);
  private productService = inject(ProductService);
  auth = inject(AuthService);

  orders = signal<Order[]>([]);
  customers = signal<Customer[]>([]);
  products = signal<Product[]>([]);
  error = signal<string | null>(null);

  form = this.fb.nonNullable.group({
    customerId: [0, Validators.min(1)],
    items: this.fb.array([this.newLine()]),
  });

  get lines() {
    return this.form.controls.items;
  }

  private newLine() {
    return this.fb.nonNullable.group({
      productId: [0, Validators.min(1)],
      quantity: [1, [Validators.required, Validators.min(1)]],
    });
  }

  canSell(): boolean {
    const role = this.auth.role();
    return role === 'Handlowiec' || role === 'Admin';
  }

  canWarehouse(): boolean {
    const role = this.auth.role();
    return role === 'Magazynier' || role === 'Admin';
  }

  canCreate(): boolean {
    return this.canSell();
  }

  ngOnInit(): void {
    this.load();
    this.customerService.getAll().subscribe((data) => this.customers.set(data));
    this.productService.getAll().subscribe((data) => this.products.set(data));
  }

  load(): void {
    this.orderService.getAll().subscribe({
      next: (data) => this.orders.set(data),
      error: () => this.error.set('Nie udało się pobrać zamówień.'),
    });
  }

  addLine(): void {
    this.lines.push(this.newLine());
  }

  removeLine(index: number): void {
    if (this.lines.length > 1) {
      this.lines.removeAt(index);
    }
  }

  create(): void {
    if (this.form.invalid) return;
    this.error.set(null);

    this.orderService.create(this.form.getRawValue()).subscribe({
      next: () => {
        this.form.reset({ customerId: 0 });
        this.lines.clear();
        this.lines.push(this.newLine());
        this.load();
      },
      error: (err) => this.error.set(err.error?.error ?? 'Nie udało się utworzyć zamówienia.'),
    });
  }

  confirm(id: number): void {
    this.runAction(this.orderService.confirm(id), 'Nie udało się potwierdzić zamówienia.');
  }

  fulfill(id: number): void {
    this.runAction(this.orderService.fulfill(id), 'Nie udało się zrealizować zamówienia.');
  }

  cancel(id: number): void {
    this.runAction(this.orderService.cancel(id), 'Nie udało się anulować zamówienia.');
  }

  private runAction(request: ReturnType<OrderService['confirm']>, fallback: string): void {
    this.error.set(null);
    request.subscribe({
      next: () => this.load(),
      error: (err) => this.error.set(err.error?.error ?? fallback),
    });
  }
}