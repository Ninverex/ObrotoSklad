using ObrotoSklad.Application.Orders;

namespace ObrotoSklad.Application.Tests;

public class CreateOrderDtoValidatorTests
{
    private readonly CreateOrderDtoValidator _validator = new();

    [Fact]
    public void Valid_Order_Passes()
    {
        var dto = new CreateOrderDto(1, new List<CreateOrderItemDto> { new(1, 5) });

        Assert.True(_validator.Validate(dto).IsValid);
    }

    [Fact]
    public void Empty_Items_Fails()
    {
        var dto = new CreateOrderDto(1, new List<CreateOrderItemDto>());

        Assert.False(_validator.Validate(dto).IsValid);
    }

    [Fact]
    public void CustomerId_Zero_Fails()
    {
        var dto = new CreateOrderDto(0, new List<CreateOrderItemDto> { new(1, 5) });

        Assert.False(_validator.Validate(dto).IsValid);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-3)]
    public void Item_NonPositiveQuantity_Fails(int quantity)
    {
        var dto = new CreateOrderDto(1, new List<CreateOrderItemDto> { new(1, quantity) });

        Assert.False(_validator.Validate(dto).IsValid);
    }

    [Fact]
    public void Item_ProductIdZero_Fails()
    {
        var dto = new CreateOrderDto(1, new List<CreateOrderItemDto> { new(0, 5) });

        Assert.False(_validator.Validate(dto).IsValid);
    }
}