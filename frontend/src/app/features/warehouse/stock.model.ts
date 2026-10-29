export type MovementType = 'In' | 'Out' | 'Reserved' | 'Released';

export interface StockItem {
  productId: number;
  productName: string;
  productSku: string;
  quantityOnHand: number;
  quantityReserved: number;
  quantityAvailable: number;
}

export interface StockMovement {
  id: number;
  productId: number;
  productName: string;
  type: MovementType;
  quantity: number;
  orderId: number | null;
  createdByUserId: string;
  createdAt: string;
}

export interface ReceiveStockRequest {
  productId: number;
  quantity: number;
}