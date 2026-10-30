import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { API_URL } from '../../core/config';
import { ReceiveStockRequest, StockItem, StockMovement } from './stock.model';

@Injectable({ providedIn: 'root' })
export class StockService {
  private http = inject(HttpClient);
  private url = `${API_URL}/stock`;

  getAll(): Observable<StockItem[]> {
    return this.http.get<StockItem[]>(this.url);
  }

  getHistory(productId: number): Observable<StockMovement[]> {
    return this.http.get<StockMovement[]>(`${this.url}/${productId}/history`);
  }

  receive(request: ReceiveStockRequest): Observable<StockItem> {
    return this.http.post<StockItem>(`${this.url}/receive`, request);
  }
}