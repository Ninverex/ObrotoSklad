using ObrotoSklad.Domain;
using ObrotoSklad.Domain.Entities;

namespace ObrotoSklad.Application.Orders;

public interface IOrderService
{
    Task<OrderDto> CreateAsync(CreateOrderDto dto, string userId);
    Task<OrderDto?> GetByIdAsync(int id);
    Task<List<OrderDto>> GetAllAsync();
    Task<OrderDto> ConfirmAsync(int orderId, string userId);
    Task<OrderDto> FulfillAsync(int orderId, string userId);
    Task<OrderDto> CancelAsync(int orderId, string userId);
}
