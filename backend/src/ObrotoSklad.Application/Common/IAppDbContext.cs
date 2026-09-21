using Microsoft.EntityFrameworkCore;
using ObrotoSklad.Domain;

namespace ObrotoSklad.Application.Common;

public interface IAppDbContext
{
    DbSet<Product> Products { get; set; }
    DbSet<StockItem> StockItems { get; set; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
