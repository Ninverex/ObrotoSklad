using ObrotoSklad.Domain;

namespace ObrotoSklad.Application.Orders;

public record OrderItemDto(int ProductId, string ProductName, int Quantity, decimal UnitPrice, decimal LineTotal);