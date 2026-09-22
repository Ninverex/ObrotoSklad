namespace ObrotoSklad.Application.Warehouse;

public record class StockItemDto(int ProductId, string ProductName, string ProductSku, int QuantityOnHand, int QuantityReserved, int QuantityAvailable);
