using System.Data;
using System.Xml;
using Microsoft.EntityFrameworkCore;
using ObrotoSklad.Application.Common;
using ObrotoSklad.Domain;
using ObrotoSklad.Domain.Entities;

namespace ObrotoSklad.Application.Warehouse;

public class StockItemService : IStockService
{
    private readonly IAppDbContext _context;

    public StockItemService(IAppDbContext context)
    {
        _context = context;
    }
    public async Task<List<StockItemDto>> GetAllStockItemDtosAsync()
    {
        var stockitems = await _context.StockItems
        .Include(s => s.Product)
        .ToListAsync(); 

        return stockitems
            .Select(stockitem => new StockItemDto(stockitem.ProductId, stockitem.Product.Name, stockitem.Product.SKU, stockitem.QuantityOnHand, stockitem.QuantityReserved, stockitem.QuantityOnHand - stockitem.QuantityReserved))
            .ToList();
    }

    public async Task<List<StockMovementDto>> GetMovementHistoryAsync(int productId)
    {
        var movements = await _context.StockMovements
        .Include(m => m.Product)
        .Where(m => m.ProductId == productId)
        .OrderByDescending(m => m.CreatedAt)
        .ToListAsync();

        return movements.Select(m => new StockMovementDto(m.Id, m.ProductId, m.Product.Name, m.Type, m.Quantity, m.OrderId, m.CreatedByUserId, m.CreatedAt)).ToList();
    }

    public async Task<StockItemDto?> GetStockItemDtoByProductIdAsync(int productId)
    {
        var stockitem = await _context.StockItems
                            .Include(s => s.Product)
                            .FirstOrDefaultAsync(s => s.ProductId == productId);
        
        if (stockitem is null)
        {
            return null;
        }
        return new StockItemDto(stockitem.ProductId, stockitem.Product.Name, stockitem.Product.SKU, stockitem.QuantityOnHand, stockitem.QuantityReserved, stockitem.QuantityOnHand - stockitem.QuantityReserved);
    }

    public async Task IssueStockAsync(IssueStockDto dto, string userId)
    {
        var stockitem = await _context.StockItems.FindAsync(dto.ProductId);
        if (stockitem is null)
        {
            throw new InvalidOperationException("Produkt o podanym id nie istnieje");
        }
        stockitem.QuantityOnHand -= dto.Quantity;
        stockitem.QuantityReserved -= dto.Quantity;

        var stockmovement = new StockMovement
        {
            ProductId = dto.ProductId,
            Type = MovementType.Out,
            Quantity = dto.Quantity,
            OrderId = dto.OrderId,
            CreatedByUserId = userId,
            CreatedAt = DateTime.UtcNow
        };

        _context.StockMovements.Add(stockmovement);

        await _context.SaveChangesAsync();
    }

    public async Task<StockItemDto> ReceiveStockAsync(ReceiveStockDto dto, string userId)
    {
        var stockitem = await _context.StockItems
                            .Include(s => s.Product)
                            .FirstOrDefaultAsync(s => s.ProductId == dto.ProductId);
        if (stockitem is null)
        {
            throw new InvalidOperationException("Produkt o podanym id nie istnieje");
        }
        stockitem.QuantityOnHand += dto.Quantity;

        var stockmovement = new StockMovement
        {
            ProductId = dto.ProductId,
            Type = MovementType.In,
            Quantity = dto.Quantity,
            CreatedByUserId = userId,
            CreatedAt = DateTime.UtcNow
        };

        _context.StockMovements.Add(stockmovement);

        await _context.SaveChangesAsync();

        return new StockItemDto(stockitem.ProductId, stockitem.Product.Name, stockitem.Product.SKU, stockitem.QuantityOnHand, stockitem.QuantityReserved, stockitem.QuantityOnHand - stockitem.QuantityReserved);
    }

    public async Task ReleaseReservationAsync(int productId, int quantity, int orderId, string userId)
    {
        var stockitem = await _context.StockItems.FindAsync(productId);
        if (stockitem is null)
        {
            throw new InvalidOperationException("Produkt o podanym id nie istnieje");
        }
        stockitem.QuantityReserved -= quantity;

        var stockmovement = new StockMovement
        {
            ProductId = productId,
            Type = MovementType.Released,
            Quantity = quantity,
            OrderId = orderId,
            CreatedByUserId = userId,
            CreatedAt = DateTime.UtcNow  
        };

        _context.StockMovements.Add(stockmovement);

        await _context.SaveChangesAsync();
    }

    public async Task ReserveStockAsync(ReserveStockDto dto, string userId)
    {
       var stockitem = await _context.StockItems
                            .FirstOrDefaultAsync(s => s.ProductId == dto.ProductId);
        if (stockitem is null)
        {
            throw new InvalidOperationException("Produkt o podanym id nie istnieje.");
        }
        var available = stockitem.QuantityOnHand - stockitem.QuantityReserved;

        if (available < dto.Quantity)
        {
            throw new InvalidOperationException("Nie jest dostępne.");
        }
        stockitem.QuantityReserved += dto.Quantity;

        var stockmovement = new StockMovement
        {
            ProductId = dto.ProductId,
            Type = MovementType.Reserved,
            Quantity = dto.Quantity,
            OrderId = dto.OrderId,
            CreatedByUserId = userId,
            CreatedAt = DateTime.UtcNow
        };

        _context.StockMovements.Add(stockmovement);

        await _context.SaveChangesAsync();

    }
}
