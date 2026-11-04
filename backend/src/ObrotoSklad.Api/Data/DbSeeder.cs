using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ObrotoSklad.Domain.Entities;
using ObrotoSklad.Infrastructure.Database;

namespace ObrotoSklad.Api.Data;

public static class DbSeeder
{
    private static readonly (string Email, string Password, string First, string Last, string Role)[] Users =
    {
        ("admin@obrotosklad.pl",      "Admin123!",      "Anna",  "Admin",      "Admin"),
        ("handlowiec@obrotosklad.pl", "Handlowiec123!", "Jan",   "Kowalski",   "Handlowiec"),
        ("magazynier@obrotosklad.pl", "Magazynier123!", "Piotr", "Nowak",       "Magazynier"),
    };

    public static async Task SeedAsync(IServiceProvider services)
    {
        var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
        var userManager = services.GetRequiredService<UserManager<User>>();
        var db = services.GetRequiredService<AppDbContext>();

        foreach (var role in new[] { "Admin", "Handlowiec", "Magazynier" })
        {
            if (!await roleManager.RoleExistsAsync(role))
                await roleManager.CreateAsync(new IdentityRole(role));
        }

        foreach (var (email, password, first, last, role) in Users)
        {
            if (await userManager.FindByEmailAsync(email) is not null) continue;

            var user = new User
            {
                UserName = email,
                Email = email,
                FirstName = first,
                LastName = last,
                CreatedAt = DateTime.UtcNow
            };

            var result = await userManager.CreateAsync(user, password);
            if (result.Succeeded)
                await userManager.AddToRoleAsync(user, role);
        }

        if (!await db.Products.AnyAsync())
        {
            var products = new[]
            {
                new Product { SKU = "MON-24", Name = "Monitor 24\"",     Description = "Monitor LED Full HD", Unit = "szt.", Price = 599.99m },
                new Product { SKU = "KLA-01", Name = "Klawiatura",       Description = "Klawiatura USB",      Unit = "szt.", Price = 89.90m },
                new Product { SKU = "MYS-01", Name = "Mysz bezprzewodowa", Description = "Mysz optyczna",     Unit = "szt.", Price = 59.00m },
            };

            foreach (var p in products)
            {
                db.Products.Add(p);
                db.StockItems.Add(new StockItem { Product = p, QuantityOnHand = 100, QuantityReserved = 0 });
            }

            await db.SaveChangesAsync();
        }
    }
}