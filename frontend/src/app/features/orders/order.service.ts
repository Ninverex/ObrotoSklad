import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { API_URL } from '../../core/config';
import { CreateOrderRequest, Order } from './order.model';

@Injectable({ providedIn: 'root' })
export class OrderService {
  private http = inject(HttpClient);
  private url = `${API_URL}/orders`;

  getAll(): Observable<Order[]> {
    return this.http.get<Order[]>(this.url);
  }

  create(request: CreateOrderRequest): Observable<Order> {
    return this.http.post<Order>(this.url, request);
  }

  confirm(id: number): Observable<Order> {
    return this.http.post<Order>(`${this.url}/${id}/confirm`, {});
  }

  fulfill(id: number): Observable<Order> {
    return this.http.post<Order>(`${this.url}/${id}/fulfill`, {});
  }

  cancel(id: number): Observable<Order> {
    return this.http.post<Order>(`${this.url}/${id}/cancel`, {});
  }
}