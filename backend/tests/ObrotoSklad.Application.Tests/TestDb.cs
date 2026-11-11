using Microsoft.EntityFrameworkCore;
using ObrotoSklad.Domain.Entities;
using ObrotoSklad.Infrastructure.Database;

namespace ObrotoSklad.Application.Tests;

public static class TestDb
{
    public static AppDbContext Create()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new AppDbContext(options);
    }

    public static async Task<Product> AddProductAsync(AppDbContext db, int onHand, decimal price = 100m)
    {
        var product = new Product
        {
            SKU = Guid.NewGuid().ToString("N")[..8],
            Name = "Produkt testowy",
            Description = "Opis",
            Unit = "szt.",
            Price = price
        };

        db.Products.Add(product);
        db.StockItems.Add(new StockItem { Product = product, QuantityOnHand = onHand, QuantityReserved = 0 });
        await db.SaveChangesAsync();
        return product;
    }

    public static async Task<Customer> AddCustomerAsync(AppDbContext db)
    {
        var customer = new Customer
        {
            Name = "Klient Testowy",
            NIP = "1234567890",
            Address = "ul. Testowa 1",
            Mail = "klient@test.pl",
            PhoneNumber = "123456789"
        };

        db.Customers.Add(customer);
        await db.SaveChangesAsync();
        return customer;
    }
}