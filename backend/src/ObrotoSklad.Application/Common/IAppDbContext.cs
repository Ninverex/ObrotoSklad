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

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
