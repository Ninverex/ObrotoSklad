using Microsoft.EntityFrameworkCore;
using ObrotoSklad.Application.Common;
using ObrotoSklad.Domain;

namespace ObrotoSklad.Application.Products;

public class ProductService : IProductService
{
    private readonly IAppDbContext _context;
    public ProductService(IAppDbContext context)
    {
        _context = context;
    }
    public async Task<ProductDto> CreateAsync(CreateProductDto dto)
    {
        var product = new Product {
             SKU = dto.SKU, 
             Name = dto.Name, 
             Description = dto.Description, 
             Unit = dto.Unit, 
             Price = dto.Price 
             };
        
        var stockItem = new StockItem
        {
            Product = product,
            QuantityOnHand = 0,
            QuantityReserved =0  
        };

        _context.Products.Add(product);
        _context.StockItems.Add(stockItem);

        await _context.SaveChangesAsync();

       return new ProductDto(product.Id, product.SKU, product.Name, product.Description, product.Unit, product.Price);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var product = await _context.Products.FindAsync(id);

        if (product is null)
        {
            return false;
        }

        _context.Products.Remove(product);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<List<ProductDto>> GetAllAsync()
    {
        var products = await _context.Products.ToListAsync();
        
        return products
            .Select(product => new ProductDto(product.Id, product.SKU, product.Name, product.Description, product.Unit, product.Price))
            .ToList();

    }

    public async Task<ProductDto?> GetByIdAsync(int id)
    {
        var product = await _context.Products.FindAsync(id);
        
        if (product is null) 
        {

            return null;
        }

        return new ProductDto(product.Id, product.SKU, product.Name, product.Description, product.Unit, product.Price);
    }

    public async Task<ProductDto?> UpdateAsync(int id, UpdateProductDto dto)
    {
        var product = await _context.Products.FindAsync(id);

        if (product is null)
        {
            return null;
        }
        
        product.Name = dto.Name;
        product.Description = dto.Description;
        product.Unit = dto.Unit;
        product.Price = dto.Price;

        await _context.SaveChangesAsync();

        return new ProductDto(product.Id, product.SKU, product.Name, product.Description, product.Unit, product.Price);
    }
}
