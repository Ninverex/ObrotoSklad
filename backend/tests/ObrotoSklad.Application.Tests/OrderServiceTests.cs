using ObrotoSklad.Application.Invoices;
using ObrotoSklad.Application.Orders;
using ObrotoSklad.Application.Warehouse;
using ObrotoSklad.Domain;
using ObrotoSklad.Infrastructure.Database;

namespace ObrotoSklad.Application.Tests;

public class OrderServiceTests
{
    private static OrderService CreateService(AppDbContext db)
    {
        var stock = new StockService(db);
        var invoices = new InvoiceService(db);
        return new OrderService(db, stock, invoices);
    }

    private static CreateOrderDto NewOrder(int customerId, int productId, int quantity) =>
        new(customerId, new List<CreateOrderItemDto> { new(productId, quantity) });

    [Fact]
    public async Task Create_StartsAsDraft_AndFreezesPrice()
    {
        await using var db = TestDb.Create();
        var product = await TestDb.AddProductAsync(db, onHand: 100, price: 100m);
        var customer = await TestDb.AddCustomerAsync(db);
        var service = CreateService(db);

        var order = await service.CreateAsync(NewOrder(customer.Id, product.Id, 2), "user-1");

        Assert.Equal(OrderStatus.Draft, order.Status);
        Assert.Equal(200m, order.TotalValue);

        product.Price = 999m;
        await db.SaveChangesAsync();

        var reloaded = await service.GetByIdAsync(order.Id);
        Assert.Equal(100m, reloaded!.Items.Single().UnitPrice);
    }

    [Fact]
    public async Task Create_UnknownCustomer_Throws()
    {
        await using var db = TestDb.Create();
        var product = await TestDb.AddProductAsync(db, onHand: 10);
        var service = CreateService(db);

        await Assert.ThrowsAsync<InvalidOperationException>(
            () => service.CreateAsync(NewOrder(999, product.Id, 1), "user-1"));
    }

    [Fact]
    public async Task Confirm_ReservesStock_AndSetsStatus()
    {
        await using var db = TestDb.Create();
        var product = await TestDb.AddProductAsync(db, onHand: 100);
        var customer = await TestDb.AddCustomerAsync(db);
        var service = CreateService(db);
        var order = await service.CreateAsync(NewOrder(customer.Id, product.Id, 10), "user-1");

        var confirmed = await service.ConfirmAsync(order.Id, "user-1");

        Assert.Equal(OrderStatus.StockReserved, confirmed.Status);
        Assert.NotNull(confirmed.ConfirmedAt);
        var stock = await db.StockItems.FindAsync(product.Id);
        Assert.Equal(10, stock!.QuantityReserved);
    }

    [Fact]
    public async Task Confirm_InsufficientStock_Throws_AndOrderStaysDraft()
    {
        await using var db = TestDb.Create();
        var product = await TestDb.AddProductAsync(db, onHand: 5);
        var customer = await TestDb.AddCustomerAsync(db);
        var service = CreateService(db);
        var order = await service.CreateAsync(NewOrder(customer.Id, product.Id, 10), "user-1");

        await Assert.ThrowsAsync<InvalidOperationException>(
            () => service.ConfirmAsync(order.Id, "user-1"));

        var reloaded = await service.GetByIdAsync(order.Id);
        Assert.Equal(OrderStatus.Draft, reloaded!.Status);
    }

    [Fact]
    public async Task Confirm_AlreadyConfirmed_Throws()
    {
        await using var db = TestDb.Create();
        var product = await TestDb.AddProductAsync(db, onHand: 100);
        var customer = await TestDb.AddCustomerAsync(db);
        var service = CreateService(db);
        var order = await service.CreateAsync(NewOrder(customer.Id, product.Id, 1), "user-1");
        await service.ConfirmAsync(order.Id, "user-1");

        await Assert.ThrowsAsync<InvalidOperationException>(
            () => service.ConfirmAsync(order.Id, "user-1"));
    }

    [Fact]
    public async Task Fulfill_WithoutConfirm_Throws()
    {
        await using var db = TestDb.Create();
        var product = await TestDb.AddProductAsync(db, onHand: 100);
        var customer = await TestDb.AddCustomerAsync(db);
        var service = CreateService(db);
        var order = await service.CreateAsync(NewOrder(customer.Id, product.Id, 1), "user-1");

        await Assert.ThrowsAsync<InvalidOperationException>(
            () => service.FulfillAsync(order.Id, "user-2"));
    }

    [Fact]
    public async Task Fulfill_IssuesStock_AndGeneratesInvoice()
    {
        await using var db = TestDb.Create();
        var product = await TestDb.AddProductAsync(db, onHand: 100);
        var customer = await TestDb.AddCustomerAsync(db);
        var service = CreateService(db);
        var order = await service.CreateAsync(NewOrder(customer.Id, product.Id, 10), "user-1");
        await service.ConfirmAsync(order.Id, "user-1");

        var fulfilled = await service.FulfillAsync(order.Id, "user-2");

        Assert.Equal(OrderStatus.Fulfilled, fulfilled.Status);
        var stock = await db.StockItems.FindAsync(product.Id);
        Assert.Equal(90, stock!.QuantityOnHand);
        Assert.Equal(0, stock.QuantityReserved);
        Assert.Single(db.Invoices);
    }

    [Fact]
    public async Task Cancel_ConfirmedOrder_ReleasesReservation()
    {
        await using var db = TestDb.Create();
        var product = await TestDb.AddProductAsync(db, onHand: 100);
        var customer = await TestDb.AddCustomerAsync(db);
        var service = CreateService(db);
        var order = await service.CreateAsync(NewOrder(customer.Id, product.Id, 10), "user-1");
        await service.ConfirmAsync(order.Id, "user-1");

        var cancelled = await service.CancelAsync(order.Id, "user-1");

        Assert.Equal(OrderStatus.Cancelled, cancelled.Status);
        var stock = await db.StockItems.FindAsync(product.Id);
        Assert.Equal(0, stock!.QuantityReserved);
    }

    [Fact]
    public async Task Cancel_FulfilledOrder_Throws()
    {
        await using var db = TestDb.Create();
        var product = await TestDb.AddProductAsync(db, onHand: 100);
        var customer = await TestDb.AddCustomerAsync(db);
        var service = CreateService(db);
        var order = await service.CreateAsync(NewOrder(customer.Id, product.Id, 1), "user-1");
        await service.ConfirmAsync(order.Id, "user-1");
        await service.FulfillAsync(order.Id, "user-2");

        await Assert.ThrowsAsync<InvalidOperationException>(
            () => service.CancelAsync(order.Id, "user-1"));
    }
}