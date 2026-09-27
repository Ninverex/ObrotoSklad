using Microsoft.AspNetCore.Http.Internal;
using Microsoft.EntityFrameworkCore;
using ObrotoSklad.Application.Common;
using ObrotoSklad.Domain;
using ObrotoSklad.Domain.Entities;
using ObrotoSklad.Domain.Enums;

namespace ObrotoSklad.Application.Invoices;

public class InvoiceService : IInvoiceService
{
    private readonly IAppDbContext _context;

    public InvoiceService(IAppDbContext context)
    {
        _context = context;
    }

    public async Task<InvoiceDto> GenerateFromOrderAsync(int orderId)
    {
        var order = await _context.Orders
                            .Include(o => o.Items).ThenInclude(i => i.Product)
                            .Include(o => o.Customer)
                            .FirstOrDefaultAsync(o => o.Id == orderId);
        
        if (order is null)
            {
                throw new InvalidOperationException("Zamówienie o podanym id nie istnieje.");
            }
        
        var invoiceExists = await _context.Invoices.AnyAsync(i => i.OrderId == orderId);
        
        if (invoiceExists)
        {
            throw new InvalidOperationException("Faktura dla tego zamówienia już istnieje.");
        }

        var invoiceNumber = $"FV-{DateTime.UtcNow:yyyyMMddHHmmssfff}";

        var invoice = new Invoice
        {
            OrderId = order.Id,
            InvoiceNumber = invoiceNumber,
            IssueDate = DateOnly.FromDateTime(DateTime.UtcNow),
            DueDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(14)),
            Status = InvoiceStatus.Issued
        };

        foreach (var item in order.Items)
        {
            var netAmount = item.Quantity * item.UnitPrice;
            var vatRate = 23m;
            var grossAmount = netAmount * 1.23m;

            var invoiceItem = new InvoiceItem
            {
                ProductId = item.ProductId,
                Quantity = item.Quantity,
                UnitPrice = item.UnitPrice,
                VatRate = vatRate,
                NetAmount = netAmount,
                GrossAmount = grossAmount
            };

            invoice.Items.Add(invoiceItem);
            
        }

        _context.Invoices.Add(invoice);
        await _context.SaveChangesAsync();

        var invoiceItemDtos = order.Items.Select(item => new InvoiceItemDto(
            item.ProductId,
            item.Product!.Name,
            item.Quantity,
            item.UnitPrice,
            23m,
            item.Quantity * item.UnitPrice,
            item.Quantity * item.UnitPrice * 1.23m)).ToList();

        return new InvoiceDto(
            invoice.Id,
            invoice.InvoiceNumber,
            invoice.OrderId,
            order.OrderNumber,
            order.Customer!.Name,
            invoice.IssueDate,
            invoice.DueDate,
            invoice.Status,
            invoiceItemDtos.Sum(i => i.NetAmount),
            invoiceItemDtos.Sum(i => i.GrossAmount - i.NetAmount),
            invoiceItemDtos.Sum(i => i.GrossAmount),
            invoiceItemDtos);
    }

    public async Task<List<InvoiceDto>> GetAllAsync()
    {
        var allInvoices = await _context.Invoices
            .Include(i => i.Items).ThenInclude(ii => ii.Product)
            .Include(i => i.Order).ThenInclude(o => o!.Customer)
            .ToListAsync();

        return allInvoices.Select(invoice => new InvoiceDto(
            invoice.Id,
            invoice.InvoiceNumber,
            invoice.OrderId,
            invoice.Order!.OrderNumber,
            invoice.Order!.Customer!.Name,
            invoice.IssueDate,
            invoice.DueDate,
            invoice.Status,
            invoice.TotalNet,
            invoice.TotalVat,
            invoice.TotalGross,
            invoice.Items.Select(ii => new InvoiceItemDto(
                ii.ProductId,
                ii.Product!.Name,
                ii.Quantity,
                ii.UnitPrice,
                ii.VatRate,
                ii.NetAmount,
                ii.GrossAmount)).ToList()
        )).ToList();
    }

    public async Task<InvoiceDto?> GetByIdAsync(int id)
    {
        var invoice = await _context.Invoices
                                .Include(i => i.Items).ThenInclude(ii => ii.Product)
                                .Include(i => i.Order).ThenInclude(o => o!.Customer)
                                .FirstOrDefaultAsync(i => i.Id == id);
        if (invoice is null) return null;

        var items = invoice.Items.Select(ii => new InvoiceItemDto(ii.ProductId, ii.Product!.Name, ii.Quantity, ii.UnitPrice, ii.VatRate, ii.NetAmount, ii.GrossAmount)).ToList();

        return new InvoiceDto(
            invoice.Id,
            invoice.InvoiceNumber,
            invoice.OrderId,
            invoice.Order!.OrderNumber,
            invoice.Order!.Customer!.Name,
            invoice.IssueDate,
            invoice.DueDate,
            invoice.Status,
            invoice.TotalNet,
            invoice.TotalVat,
            invoice.TotalGross,
            items);
    }
}
