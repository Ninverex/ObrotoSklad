using FluentValidation;

namespace ObrotoSklad.Application.Warehouse;

public class IssueStockDtoValidator : AbstractValidator<IssueStockDto>
{
    public IssueStockDtoValidator()
    {
        RuleFor(r => r.ProductId)
            .GreaterThan(0);
        
        RuleFor(r => r.Quantity)
            .GreaterThan(0);
        
        RuleFor(r => r.OrderId)
            .GreaterThan(0);
    }
}
