using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ObrotoSklad.Application.Warehouse;
using ObrotoSklad.Domain;

namespace ObrotoSklad.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StockController : ControllerBase
    {
        public readonly IStockService _stockService;

        public StockController(IStockService stockService)
        {
            _stockService = stockService;
        }
    
    [HttpGet]
    public async Task<ActionResult<List<StockItemDto>>> GetAllStockItemsAsync()
        {
            var stockItem = await _stockService.GetAllStockItemDtosAsync();
            return Ok(stockItem);
        }

    [HttpGet("{productId}")]
    public async Task<ActionResult<StockItemDto>> GetOneStockItemByIdAsync(int productId)
        {
            var stockItem = await _stockService.GetStockItemDtoByProductIdAsync(productId);

            if (stockItem is null)
            {
                return NotFound();
            }
            return Ok(stockItem);
        }
    
    [HttpGet("{productId}/history")]
    public async Task<ActionResult<List<StockItemDto>>> GetStockItemMovementHistory(int productId)
        {
            var stockMovementHistory = await _stockService.GetMovementHistoryAsync(productId);
            return Ok(stockMovementHistory);
        }

    [HttpPost("receive")]
    [Authorize]
    public async Task<ActionResult<StockItemDto>> ReceiveStock(ReceiveStockDto dto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            
            var stockItem = await _stockService.ReceiveStockAsync(dto, userId);

            return Ok(stockItem);
        }    

    [HttpPost("reserve")]
    [Authorize]
    public async Task<ActionResult> ReserveStock(ReserveStockDto dto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;

            await _stockService.ReserveStockAsync(dto, userId);

            return NoContent();
        }
    
    [HttpPost("release")]
    [Authorize]
    public async Task<ActionResult> ReleaseReservation(ReleaseReservationDto dto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;

            await _stockService.ReleaseReservationAsync(dto.ProductId, dto.Quantity, dto.OrderId, userId);

            return NoContent();
        }

    [HttpPost("issue")]
    [Authorize]
    public async Task<ActionResult> IssueStock(IssueStockDto dto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;

            await _stockService.IssueStockAsync(dto, userId);

            return NoContent();
        }
    }
}

