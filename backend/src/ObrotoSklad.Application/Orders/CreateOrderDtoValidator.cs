using FluentValidation;

namespace ObrotoSklad.Application.Orders;

public class CreateOrderDtoValidator : AbstractValidator<CreateOrderDto>
{
    public CreateOrderDtoValidator()
    {
        RuleFor(o => o.CustomerId).GreaterThan(0);

        RuleFor(o => o.Items).NotEmpty();

        RuleForEach(o => o.Items).ChildRules(item =>
        {
            item.RuleFor(i => i.ProductId).GreaterThan(0);
            item.RuleFor(i => i.Quantity).GreaterThan(0);
        });
    }
}
