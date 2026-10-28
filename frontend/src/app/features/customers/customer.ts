import { Component, inject, signal, OnInit } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { RouterLink } from '@angular/router';
import { AuthService } from '../../core/auth/auth.service';
import { CustomerService } from './customer.service';
import { Customer } from './customer.model';

@Component({
  selector: 'app-customers',
  imports: [ReactiveFormsModule, RouterLink],
  template: `
    <header class="top">
      <h1>Klienci</h1>
      <div>
        <a routerLink="/products">Produkty</a>
        <span>Rola: {{ auth.role() }}</span>
        <button class="secondary" (click)="auth.logout()">Wyloguj</button>
      </div>
    </header>

    @if (error()) {
      <div class="error">{{ error() }}</div>
    }

    <form class="add" [formGroup]="form" (ngSubmit)="add()">
      <input placeholder="Nazwa" formControlName="name" />
      <input placeholder="NIP (10 cyfr)" formControlName="nip" maxlength="10" />
      <input placeholder="Adres" formControlName="address" />
      <input placeholder="Email" type="email" formControlName="mail" />
      <input placeholder="Telefon" formControlName="phoneNumber" />
      <button type="submit" [disabled]="form.invalid">Dodaj</button>
    </form>

    <table>
      <thead>
        <tr>
          <th>ID</th><th>Nazwa</th><th>NIP</th><th>Adres</th><th>Email</th><th>Telefon</th><th></th>
        </tr>
      </thead>
      <tbody>
        @for (c of customers(); track c.id) {
          <tr>
            <td>{{ c.id }}</td>
            <td>{{ c.name }}</td>
            <td>{{ c.nip }}</td>
            <td>{{ c.address }}</td>
            <td>{{ c.mail }}</td>
            <td>{{ c.phoneNumber }}</td>
            <td><button class="danger" (click)="remove(c.id)">Usuń</button></td>
          </tr>
        } @empty {
          <tr><td colspan="7">Brak klientów.</td></tr>
        }
      </tbody>
    </table>
  `,
  styles: `
    :host { display: block; max-width: 1000px; margin: 24px auto; padding: 0 16px; }
    .top { display: flex; justify-content: space-between; align-items: center; }
    .top span, .top a { margin-right: 12px; color: #6b7280; }
    .add { display: flex; gap: 8px; margin: 16px 0; flex-wrap: wrap; }
    input { padding: 8px 10px; border: 1px solid #d1d5db; border-radius: 6px; }
    button { padding: 8px 14px; border: none; border-radius: 6px; background: #2563eb; color: #fff; cursor: pointer; }
    button:disabled { background: #93c5fd; cursor: not-allowed; }
    button.secondary { background: #6b7280; }
    button.danger { background: #dc2626; }
    table { width: 100%; border-collapse: collapse; }
    th, td { text-align: left; padding: 10px; border-bottom: 1px solid #e5e7eb; }
    .error { padding: 10px; background: #fee2e2; color: #b91c1c; border-radius: 6px; margin: 12px 0; }
  `,
})
export class Customers implements OnInit {
  private fb = inject(FormBuilder);
  private service = inject(CustomerService);
  auth = inject(AuthService);

  customers = signal<Customer[]>([]);
  error = signal<string | null>(null);

  form = this.fb.nonNullable.group({
    name: ['', [Validators.required, Validators.maxLength(200)]],
    nip: ['', [Validators.required, Validators.pattern(/^\d{10}$/)]],
    address: ['', Validators.required],
    mail: ['', [Validators.required, Validators.email]],
    phoneNumber: ['', Validators.required],
  });

  ngOnInit(): void {
    this.load();
  }

  load(): void {
    this.service.getAll().subscribe({
      next: (data) => this.customers.set(data),
      error: () => this.error.set('Nie udało się pobrać klientów.'),
    });
  }

  add(): void {
    if (this.form.invalid) return;
    this.error.set(null);

    this.service.create(this.form.getRawValue()).subscribe({
      next: () => {
        this.form.reset();
        this.load();
      },
      error: () => this.error.set('Nie udało się dodać klienta (sprawdź poprawność danych).'),
    });
  }

  remove(id: number): void {
    this.error.set(null);
    this.service.delete(id).subscribe({
      next: () => this.load(),
      error: () => this.error.set('Nie można usunąć klienta (może mieć zamówienia).'),
    });
  }
}