using ObrotoSklad.Application.Common;
using ObrotoSklad.Application.Warehouse;
using ObrotoSklad.Domain;
using ObrotoSklad.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using ObrotoSklad.Application.Invoices;

namespace ObrotoSklad.Application.Orders;

public class OrderService : IOrderService
{
    private readonly IAppDbContext _context;
    private readonly IStockService _stockService;
    private readonly IInvoiceService _invoiceService;

    public OrderService(IAppDbContext context, IStockService stockService, IInvoiceService invoiceService)
    {
        _context = context;
        _stockService = stockService;
        _invoiceService = invoiceService;
    }

    public async Task<OrderDto> CancelAsync(int orderId, string userId)
    {
        var order = await _context.Orders
                                .Include(o => o.Items)
                                .Include(o => o.Customer)
                                .FirstOrDefaultAsync(o => o.Id == orderId);

         if (order is null)
            {
                throw new InvalidOperationException("Zamówienie o podanym id nie istnieje.");
            }

        if (order.Status == OrderStatus.Fullfiled || order.Status == OrderStatus.Canceled)
            {
                throw new InvalidOperationException("Nie można anulować zrealizowanego lub już anulowanego zamówienia.");
            }
        
        if (order.Status == OrderStatus.StockReserved)
        {
            foreach (var item in order.Items)
            {
                await _stockService.ReleaseReservationAsync(item.ProductId, item.Quantity, order.Id, userId);
            }
        }

        order.Status = OrderStatus.Canceled;

        await _context.SaveChangesAsync();

        var orderItemDtos = order.Items.Select(i => new OrderItemDto(i.ProductId, i.Product!.Name, i.Quantity, i.UnitPrice, i.Quantity * i.UnitPrice)).ToList();

        return new OrderDto(
            order.Id,
            order.OrderNumber,
            order.CustomerId,
            order.Customer!.Name,
            order.Status,
            order.CreatedByUserId,
            order.CreatedAt,
            order.ConfirmedAt,
            orderItemDtos,
            orderItemDtos.Sum(i => i.LineTotal));
    }

    public async Task<OrderDto> ConfirmAsync(int orderId, string userId)
    {
        var order = await _context.Orders 
                            .Include(o => o.Items).ThenInclude(i => i.Product)
                            .Include(o => o.Customer)
                            .FirstOrDefaultAsync(o => o.Id == orderId);
        
        if (order is null)
        {
            throw new InvalidOperationException("Zamówienie o podanym id nie istnieje.");
        }

        if (order.Status != OrderStatus.Draft)
        {
            throw new InvalidOperationException("Nie można potwierdzić zamówienia w tym stanie");
        }

        foreach (var item in order.Items)
        {
            await _stockService.ReserveStockAsync(new ReserveStockDto(item.ProductId, item.Quantity, item.OrderId), userId);
        }

        order.Status = OrderStatus.StockReserved;

        order.ConfirmedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

         var orderItemDtos = order.Items.Select(i => new OrderItemDto(i.ProductId, i.Product!.Name, i.Quantity, i.UnitPrice, i.Quantity * i.UnitPrice)).ToList();

        return new OrderDto(
            order.Id,
            order.OrderNumber,
            order.CustomerId,
            order.Customer!.Name,
            order.Status,
            order.CreatedByUserId,
            order.CreatedAt,
            order.ConfirmedAt,
            orderItemDtos,
            orderItemDtos.Sum(i => i.LineTotal));
    }

    public async Task<OrderDto> CreateAsync(CreateOrderDto dto, string userId)
    {   
        var customer = await _context.Customers.FindAsync(dto.CustomerId);

        if (customer is null)
        {
            throw new InvalidOperationException("Klient o podanym id nie istnieje.");
        }

        var orderNumber = $"ORD-{DateTime.UtcNow:yyyyMMddHHmmssfff}";

        var order = new Order { CustomerId = dto.CustomerId, OrderNumber = orderNumber, CreatedByUserId = userId, Status = OrderStatus.Draft, CreatedAt = DateTime.UtcNow };

        var orderItemDtos = new List<OrderItemDto>();

        foreach (var item in dto.Items)
        {
            var product = await _context.Products.FindAsync(item.ProductId);

            if (product is null)
            {
                throw new InvalidOperationException($"Produkt o id {item.ProductId} nie istnieje.");
            }

            var orderItem = new OrderItem
            {
                ProductId = item.ProductId,
                Quantity = item.Quantity,
                UnitPrice = product.Price
            };

            order.Items.Add(orderItem);

            orderItemDtos.Add(new OrderItemDto(
                product.Id,
                product.Name,
                item.Quantity,
                product.Price,
                item.Quantity * product.Price));
        }

        _context.Orders.Add(order);
        await _context.SaveChangesAsync();

        return new OrderDto(
            order.Id,
            order.OrderNumber,
            order.CustomerId,
            customer.Name,
            order.Status,
            order.CreatedByUserId,
            order.CreatedAt,
            order.ConfirmedAt,
            orderItemDtos,
            orderItemDtos.Sum(i => i.LineTotal));
    }

    public async Task<OrderDto> FulfillAsync(int orderId, string userId)
    {
        var order = await _context.Orders
                            .Include(o => o.Items)
                            .FirstOrDefaultAsync(o => o.Id == orderId);

        if (order is null)
        {
            throw new InvalidOperationException("Zamówienie o podanym id nie istnieje.");
        }

        if (order.Status != OrderStatus.StockReserved)
        {
            throw new InvalidOperationException("Nie można zrealizować zamówienia w tym stanie");
        }

        foreach (var item in order.Items)
        {
            await _stockService.IssueStockAsync(new IssueStockDto(item.ProductId, item.Quantity, order.Id), userId);
        }

        order.Status = OrderStatus.Fullfiled;

        await _context.SaveChangesAsync();

        await _invoiceService.GenerateFromOrderAsync(order.Id);

        var orderItemDtos = order.Items.Select(i => new OrderItemDto(i.ProductId, i.Product!.Name, i.Quantity, i.UnitPrice, i.Quantity * i.UnitPrice)).ToList();

        return new OrderDto(
            order.Id,
            order.OrderNumber,
            order.CustomerId,
            order.Customer!.Name,
            order.Status,
            order.CreatedByUserId,
            order.CreatedAt,
            order.ConfirmedAt,
            orderItemDtos,
            orderItemDtos.Sum(i => i.LineTotal));
    }

    public async Task<List<OrderDto>> GetAllAsync()
    {
        var allOrders = await _context.Orders
                                    .Include(o => o.Customer)
                                    .Include(o => o.Items).ThenInclude(i => i.Product)
                                    .ToListAsync();
        
        return allOrders.Select(order => new OrderDto(
                        order.Id,
                        order.OrderNumber,
                        order.CustomerId,
                        order.Customer!.Name,
                        order.Status,
                        order.CreatedByUserId,
                        order.CreatedAt,
                        order.ConfirmedAt,
                        order.Items.Select(i => new OrderItemDto(i.ProductId, i.Product!.Name, i.Quantity, i.UnitPrice, i.Quantity * i.UnitPrice)).ToList(),
                        order.Items.Sum(i => i.Quantity * i.UnitPrice)
                    )).ToList();
    }

    public async Task<OrderDto?> GetByIdAsync(int id)
    {
        var order = await _context.Orders
                                .Include(o => o.Customer)
                                .Include(o => o.Items).ThenInclude(i => i.Product)
                                .FirstOrDefaultAsync(o => o.Id == id);
        
        if (order is null)
        {
            return null;
        }

        var orderItemDtos = order.Items.Select(i => new OrderItemDto(i.ProductId, i.Product!.Name, i.Quantity, i.UnitPrice, i.Quantity * i.UnitPrice)).ToList();

        return new OrderDto(
            order.Id,
            order.OrderNumber,
            order.CustomerId,
            order.Customer!.Name,
            order.Status,
            order.CreatedByUserId,
            order.CreatedAt,
            order.ConfirmedAt,
            orderItemDtos,
            orderItemDtos.Sum(i => i.LineTotal));
    }
}
