using ObrotoSklad.Domain;

namespace ObrotoSklad.Application.Warehouse;

public record class StockMovementDto(int Id, int ProductId, string ProductName, MovementType Type, int Quantity, int? OrderId, string CreatedByUserId, DateTime CreatedAt);