using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ObrotoSklad.Application.Orders;

namespace ObrotoSklad.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        public readonly IOrderService _orderService;

        public OrdersController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        [HttpPost]
        [Authorize(Roles = "Handlowiec,Admin")]
        public async Task <ActionResult<OrderDto>> CreateOrder(CreateOrderDto dto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var order = await _orderService.CreateAsync(dto, userId!);

            return CreatedAtAction(nameof(GetOrderById), new { orderId = order.Id}, order);
        }
        
        [HttpGet]
        [Authorize]
        public async Task <ActionResult<List<OrderDto>>> GetAllOrders()
        {
            var order = await _orderService.GetAllAsync();
            return Ok(order);
        }
        
        [HttpGet("{orderId}")]
        [Authorize]
        public async Task <ActionResult<OrderDto>> GetOrderById(int orderId)
        {
            var order = await _orderService.GetByIdAsync(orderId);

            if (order is null)
            {
                return NotFound();
            }
            return Ok(order);
        }

        [HttpPost("{orderId}/confirm")]
        [Authorize(Roles ="Handlowiec,Admin")]
        public async Task <ActionResult<OrderDto>> ConfirmOrder(int orderId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            
            var order = await _orderService.ConfirmAsync(orderId, userId);

            return Ok(order);
        }

        [HttpPost("{orderId}/fulfill")]
        [Authorize(Roles = "Magazynier,Admin")]
        public async Task<ActionResult<OrderDto>> FulfillOrder(int orderId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;

            var order = await _orderService.FulfillAsync(orderId, userId);

            return Ok(order);
        }

        [HttpPost("{orderId}/cancel")]
        [Authorize]
        public async Task<ActionResult<OrderDto>> CancelOrder(int orderId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;

            var order = await _orderService.CancelAsync(orderId, userId);

            return Ok(order);
        }
    }
}
