import { Component, inject, signal } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { AuthService } from '../../../core/auth/auth.service';

@Component({
  selector: 'app-login',
  imports: [ReactiveFormsModule, RouterLink],
  template: `
    <div class="wrapper">
      <form class="card" [formGroup]="form" (ngSubmit)="submit()">
        <h1>ObrotoSkład</h1>
        <p class="subtitle">Zaloguj się do systemu</p>

        <label>
          Email
          <input type="email" formControlName="email" autocomplete="username" />
        </label>
        @if (form.controls.email.touched && form.controls.email.invalid) {
          <span class="field-error">Podaj poprawny adres email.</span>
        }

        <label>
          Hasło
          <input type="password" formControlName="password" autocomplete="current-password" />
        </label>
        @if (form.controls.password.touched && form.controls.password.invalid) {
          <span class="field-error">Hasło jest wymagane.</span>
        }

        @if (error()) {
          <div class="error">{{ error() }}</div>
        }

        <button type="submit" [disabled]="form.invalid || loading()">
          {{ loading() ? 'Logowanie...' : 'Zaloguj się' }}
        </button>

        <p class="footer">
          Nie masz konta? <a routerLink="/register">Zarejestruj się</a>
        </p>
      </form>
    </div>
  `,
  styles: `
    .wrapper {
      min-height: 100vh;
      display: flex;
      align-items: center;
      justify-content: center;
      background: #f3f4f6;
    }
    .card {
      width: 100%;
      max-width: 380px;
      padding: 32px;
      background: #fff;
      border-radius: 12px;
      box-shadow: 0 4px 24px rgba(0, 0, 0, 0.08);
      display: flex;
      flex-direction: column;
      gap: 12px;
    }
    h1 { margin: 0; font-size: 1.6rem; }
    .subtitle { margin: 0 0 8px; color: #6b7280; }
    label { display: flex; flex-direction: column; gap: 4px; font-size: 0.9rem; }
    input {
      padding: 10px 12px;
      border: 1px solid #d1d5db;
      border-radius: 8px;
      font-size: 1rem;
    }
    input:focus { outline: 2px solid #2563eb; border-color: transparent; }
    button {
      margin-top: 8px;
      padding: 12px;
      border: none;
      border-radius: 8px;
      background: #2563eb;
      color: #fff;
      font-size: 1rem;
      cursor: pointer;
    }
    button:disabled { background: #93c5fd; cursor: not-allowed; }
    .error {
      padding: 10px;
      background: #fee2e2;
      color: #b91c1c;
      border-radius: 8px;
      font-size: 0.9rem;
    }
    .field-error { color: #b91c1c; font-size: 0.8rem; }
    .footer { text-align: center; margin: 8px 0 0; font-size: 0.9rem; }
  `,
})
export class Login {
  private fb = inject(FormBuilder);
  private auth = inject(AuthService);
  private router = inject(Router);

  loading = signal(false);
  error = signal<string | null>(null);

  form = this.fb.nonNullable.group({
    email: ['', [Validators.required, Validators.email]],
    password: ['', Validators.required],
  });

  submit(): void {
    if (this.form.invalid) return;

    this.loading.set(true);
    this.error.set(null);

    this.auth.login(this.form.getRawValue()).subscribe({
      next: () => this.router.navigate(['/products']),
      error: (err) => {
        this.loading.set(false);
        this.error.set(
          err.status === 401 ? 'Nieprawidłowy email lub hasło.' : 'Nie udało się połączyć z serwerem.'
        );
      },
    });
  }
}