using FluentValidation;

namespace ObrotoSklad.Application.Products;

public class UpdateProductDtoValidator : AbstractValidator<UpdateProductDto>
{
    public UpdateProductDtoValidator()
    {
        RuleFor(p => p.Name)
            .NotEmpty().WithMessage("Nazwa jest wymagana.")
            .MaximumLength(200).WithMessage("Nazwa nie może przekraczać 200 znaków.");

        RuleFor(p => p.Unit)
            .NotEmpty().WithMessage("Jednostka jest wymagana.");

        RuleFor(p => p.Price)
            .GreaterThan(0).WithMessage("Cena musi być większa od zera.");
            

        RuleFor(p => p.Description)
            .MaximumLength(1000).WithMessage("Opis nie może przekraczać 1000 znaków.");
    }
}
