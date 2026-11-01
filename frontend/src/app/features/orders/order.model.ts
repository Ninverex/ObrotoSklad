export type OrderStatus =
  | 'Draft'
  | 'Confirmed'
  | 'StockReserved'
  | 'OnHold'
  | 'Fulfilled'
  | 'Invoiced'
  | 'Cancelled';

export interface OrderItem {
  productId: number;
  productName: string;
  quantity: number;
  unitPrice: number;
  lineTotal: number;
}

export interface Order {
  id: number;
  orderNumber: string;
  customerId: number;
  customerName: string;
  status: OrderStatus;
  createdByUserId: string;
  createdAt: string;
  confirmedAt: string | null;
  items: OrderItem[];
  totalValue: number;
}

export interface CreateOrderItemRequest {
  productId: number;
  quantity: number;
}

export interface CreateOrderRequest {
  customerId: number;
  items: CreateOrderItemRequest[];
}