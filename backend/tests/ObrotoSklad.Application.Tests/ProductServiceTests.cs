using ObrotoSklad.Application.Products;

namespace ObrotoSklad.Application.Tests;

public class ProductServiceTests
{
    [Fact]
    public async Task Create_AlsoCreatesEmptyStockItem()
    {
        await using var db = TestDb.Create();
        var service = new ProductService(db);

        var result = await service.CreateAsync(new CreateProductDto("SKU-1", "Monitor", "Opis", "szt.", 599.99m));

        Assert.True(result.Id > 0);
        var stock = Assert.Single(db.StockItems);
        Assert.Equal(result.Id, stock.ProductId);
        Assert.Equal(0, stock.QuantityOnHand);
        Assert.Equal(0, stock.QuantityReserved);
    }

    [Fact]
    public async Task GetById_UnknownId_ReturnsNull()
    {
        await using var db = TestDb.Create();
        var service = new ProductService(db);

        Assert.Null(await service.GetByIdAsync(999));
    }

    [Fact]
    public async Task Update_ChangesFields_ButNotSku()
    {
        await using var db = TestDb.Create();
        var service = new ProductService(db);
        var created = await service.CreateAsync(new CreateProductDto("SKU-1", "Stara", "Opis", "szt.", 10m));

        var updated = await service.UpdateAsync(created.Id, new UpdateProductDto("Nowa", "Nowy opis", "kg", 20m));

        Assert.Equal("Nowa", updated!.Name);
        Assert.Equal(20m, updated.Price);
        Assert.Equal("SKU-1", updated.SKU);
    }

    [Fact]
    public async Task Delete_UnknownId_ReturnsFalse()
    {
        await using var db = TestDb.Create();
        var service = new ProductService(db);

        Assert.False(await service.DeleteAsync(999));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-5)]
    public void Validator_RejectsNonPositivePrice(double price)
    {
        var validator = new CreateProductDtoValidator();

        var result = validator.Validate(new CreateProductDto("SKU", "Nazwa", "Opis", "szt.", (decimal)price));

        Assert.False(result.IsValid);
    }

    [Fact]
    public void Validator_AcceptsCorrectProduct()
    {
        var validator = new CreateProductDtoValidator();

        var result = validator.Validate(new CreateProductDto("SKU", "Nazwa", "Opis", "szt.", 10m));

        Assert.True(result.IsValid);
    }
}