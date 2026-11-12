using ObrotoSklad.Application.Warehouse;
using ObrotoSklad.Domain;

namespace ObrotoSklad.Application.Tests;

public class StockServiceTests
{
    [Fact]
    public async Task Receive_IncreasesStock_AndLogsMovement()
    {
        await using var db = TestDb.Create();
        var product = await TestDb.AddProductAsync(db, onHand: 0);
        var service = new StockService(db);

        var result = await service.ReceiveStockAsync(new ReceiveStockDto(product.Id, 50), "user-1");

        Assert.Equal(50, result.QuantityOnHand);
        var movement = Assert.Single(db.StockMovements);
        Assert.Equal(MovementType.In, movement.Type);
        Assert.Equal(50, movement.Quantity);
    }

    [Fact]
    public async Task Receive_UnknownProduct_Throws()
    {
        await using var db = TestDb.Create();
        var service = new StockService(db);

        await Assert.ThrowsAsync<InvalidOperationException>(
            () => service.ReceiveStockAsync(new ReceiveStockDto(999, 5), "user-1"));
    }

    [Fact]
    public async Task Reserve_WithEnoughStock_IncreasesReserved_ButNotOnHand()
    {
        await using var db = TestDb.Create();
        var product = await TestDb.AddProductAsync(db, onHand: 100);
        var service = new StockService(db);

        await service.ReserveStockAsync(new ReserveStockDto(product.Id, 30, 1), "user-1");

        var stock = await db.StockItems.FindAsync(product.Id);
        Assert.Equal(100, stock!.QuantityOnHand);
        Assert.Equal(30, stock.QuantityReserved);
    }

    [Fact]
    public async Task Reserve_MoreThanAvailable_Throws_AndChangesNothing()
    {
        await using var db = TestDb.Create();
        var product = await TestDb.AddProductAsync(db, onHand: 10);
        var service = new StockService(db);

        await Assert.ThrowsAsync<InvalidOperationException>(
            () => service.ReserveStockAsync(new ReserveStockDto(product.Id, 11, 1), "user-1"));

        var stock = await db.StockItems.FindAsync(product.Id);
        Assert.Equal(0, stock!.QuantityReserved);
        Assert.Empty(db.StockMovements);
    }

    [Fact]
    public async Task Reserve_CannotReserveSameStockTwice()
    {
        await using var db = TestDb.Create();
        var product = await TestDb.AddProductAsync(db, onHand: 10);
        var service = new StockService(db);

        await service.ReserveStockAsync(new ReserveStockDto(product.Id, 8, 1), "user-1");

        // zostały tylko 2 dostępne sztuki
        await Assert.ThrowsAsync<InvalidOperationException>(
            () => service.ReserveStockAsync(new ReserveStockDto(product.Id, 5, 2), "user-2"));
    }

    [Fact]
    public async Task Release_DecreasesReserved()
    {
        await using var db = TestDb.Create();
        var product = await TestDb.AddProductAsync(db, onHand: 100);
        var service = new StockService(db);
        await service.ReserveStockAsync(new ReserveStockDto(product.Id, 30, 1), "user-1");

        await service.ReleaseReservationAsync(product.Id, 30, 1, "user-1");

        var stock = await db.StockItems.FindAsync(product.Id);
        Assert.Equal(0, stock!.QuantityReserved);
        Assert.Equal(100, stock.QuantityOnHand);
    }

    [Fact]
    public async Task Issue_DecreasesBothCounters()
    {
        await using var db = TestDb.Create();
        var product = await TestDb.AddProductAsync(db, onHand: 100);
        var service = new StockService(db);
        await service.ReserveStockAsync(new ReserveStockDto(product.Id, 30, 1), "user-1");

        await service.IssueStockAsync(new IssueStockDto(product.Id, 30, 1), "user-1");

        var stock = await db.StockItems.FindAsync(product.Id);
        Assert.Equal(70, stock!.QuantityOnHand);
        Assert.Equal(0, stock.QuantityReserved);
    }
}