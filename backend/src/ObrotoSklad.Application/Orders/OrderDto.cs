using ObrotoSklad.Domain;

namespace ObrotoSklad.Application.Orders;

public record OrderDto(int Id,string OrderNumber, int CustomerId, string CustomerName, OrderStatus Status, string CreatedByUserId, DateTime CreatedAt, DateTime? ConfirmedAt, List<OrderItemDto> Items, decimal TotalValue);