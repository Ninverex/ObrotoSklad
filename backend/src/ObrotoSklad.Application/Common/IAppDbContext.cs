using Microsoft.EntityFrameworkCore;
using ObrotoSklad.Domain;
using ObrotoSklad.Domain.Entities;

namespace ObrotoSklad.Application.Common;

public interface IAppDbContext
{
    DbSet<Product> Products { get; set; }
    DbSet<StockItem> StockItems { get; set; }
    DbSet<Customer> Customers {get; set; }
    DbSet<StockMovement> StockMovements { get; set; }
    DbSet<Order> Orders { get; set; }
    DbSet<OrderItem> OrderItems { get; set; }
    DbSet<Invoice> Invoices { get; set; }
    DbSet<InvoiceItem> InvoiceItems { get; set; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
