using System;
using System.Data;
using FluentValidation;

namespace ObrotoSklad.Application.Warehouse;

public class ReceiveStockDtoValidator : AbstractValidator<ReceiveStockDto>
{
    public ReceiveStockDtoValidator()
    {
        RuleFor(r => r.ProductInt)
            .GreaterThan(0);

        RuleFor(r => r.Quantity)
            .GreaterThan(0);
    }
}
