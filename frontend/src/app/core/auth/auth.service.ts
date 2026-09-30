import { Injectable, signal, computed, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Router } from '@angular/router';
import { Observable, tap } from 'rxjs';
import { API_URL } from '../config';
import { AuthResponse, LoginRequest, RegisterRequest } from '../models/auth.model';

const STORAGE_KEY = 'auth_session';

@Injectable({
  providedIn: 'root',
})
export class AuthService {
  private http = inject(HttpClient);
  private router = inject(Router);

  private session = signal<AuthResponse | null>(null);

  isLoggedIn = computed(() => this.session() !== null);
  role = computed(() => this.session()?.role ?? null);
  token = computed(() => this.session()?.token ?? null);

  constructor() {
    const stored = localStorage.getItem(STORAGE_KEY);

    if (stored) {
      try {
        const parsed: AuthResponse = JSON.parse(stored);
        this.session.set(parsed);
      } catch {
        localStorage.removeItem(STORAGE_KEY);
      }
    }
  }

  login(request: LoginRequest): Observable<AuthResponse> {
    return this.http
      .post<AuthResponse>(`${API_URL}/auth/login`, request)
      .pipe(tap((response) => this.setSession(response)));
  }

  register(request: RegisterRequest): Observable<AuthResponse> {
    return this.http
      .post<AuthResponse>(`${API_URL}/auth/register`, request)
      .pipe(tap((response) => this.setSession(response)));
  }

  logout(): void {
    this.session.set(null);
    localStorage.removeItem(STORAGE_KEY);
    this.router.navigate(['/login']);
  }

  private setSession(response: AuthResponse): void {
    this.session.set(response);
    localStorage.setItem(STORAGE_KEY, JSON.stringify(response));
  }
}
