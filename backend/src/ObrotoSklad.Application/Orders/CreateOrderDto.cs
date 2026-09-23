using System.Data.Common;

namespace ObrotoSklad.Application.Orders;

public record class CreateOrderDto(int CustomerId, List<CreateOrderItemDto> Items);
