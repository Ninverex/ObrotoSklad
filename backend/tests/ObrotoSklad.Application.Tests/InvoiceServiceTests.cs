using ObrotoSklad.Application.Invoices;
using ObrotoSklad.Application.Orders;

namespace ObrotoSklad.Application.Tests;

public class InvoiceServiceTests
{
    private static CreateOrderDto NewOrder(int customerId, int productId, int quantity) =>
        new(customerId, new List<CreateOrderItemDto> { new(productId, quantity) });

    [Fact]
    public async Task Generate_CalculatesVat23_Percent()
    {
        await using var db = TestDb.Create();
        var product = await TestDb.AddProductAsync(db, onHand: 100, price: 100m);
        var customer = await TestDb.AddCustomerAsync(db);
        var orders = new OrderService(db, new Warehouse.StockService(db), new InvoiceService(db));
        var order = await orders.CreateAsync(NewOrder(customer.Id, product.Id, 2), "user-1");
        var service = new InvoiceService(db);

        var invoice = await service.GenerateFromOrderAsync(order.Id);

        Assert.Equal(200m, invoice.TotalNet);
        Assert.Equal(46m, invoice.TotalVat);
        Assert.Equal(246m, invoice.TotalGross);
        Assert.Equal(23m, invoice.Items.Single().VatRate);
    }

    [Fact]
    public async Task Generate_SavesCorrectTotals()
    {
        await using var db = TestDb.Create();
        var product = await TestDb.AddProductAsync(db, onHand: 100, price: 100m);
        var customer = await TestDb.AddCustomerAsync(db);
        var orders = new OrderService(db, new Warehouse.StockService(db), new InvoiceService(db));
        var order = await orders.CreateAsync(NewOrder(customer.Id, product.Id, 2), "user-1");

        await new InvoiceService(db).GenerateFromOrderAsync(order.Id);

        var saved = db.Invoices.Single();
        Assert.Equal(200m, saved.TotalNet);
        Assert.Equal(246m, saved.TotalGross);
    }

    [Fact]
    public async Task Generate_SetsDueDate14DaysAfterIssue()
    {
        await using var db = TestDb.Create();
        var product = await TestDb.AddProductAsync(db, onHand: 100);
        var customer = await TestDb.AddCustomerAsync(db);
        var orders = new OrderService(db, new Warehouse.StockService(db), new InvoiceService(db));
        var order = await orders.CreateAsync(NewOrder(customer.Id, product.Id, 1), "user-1");

        var invoice = await new InvoiceService(db).GenerateFromOrderAsync(order.Id);

        Assert.Equal(invoice.IssueDate.AddDays(14), invoice.DueDate);
    }

    [Fact]
    public async Task Generate_Twice_ForSameOrder_Throws()
    {
        await using var db = TestDb.Create();
        var product = await TestDb.AddProductAsync(db, onHand: 100);
        var customer = await TestDb.AddCustomerAsync(db);
        var orders = new OrderService(db, new Warehouse.StockService(db), new InvoiceService(db));
        var order = await orders.CreateAsync(NewOrder(customer.Id, product.Id, 1), "user-1");
        var service = new InvoiceService(db);
        await service.GenerateFromOrderAsync(order.Id);

        await Assert.ThrowsAsync<InvalidOperationException>(
            () => service.GenerateFromOrderAsync(order.Id));
    }

    [Fact]
    public async Task Generate_UnknownOrder_Throws()
    {
        await using var db = TestDb.Create();
        var service = new InvoiceService(db);

        await Assert.ThrowsAsync<InvalidOperationException>(
            () => service.GenerateFromOrderAsync(999));
    }

    [Fact]
    public async Task GetById_UnknownId_ReturnsNull()
    {
        await using var db = TestDb.Create();

        Assert.Null(await new InvoiceService(db).GetByIdAsync(999));
    }
}