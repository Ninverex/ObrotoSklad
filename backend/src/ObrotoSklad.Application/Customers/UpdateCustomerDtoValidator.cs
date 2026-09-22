using FluentValidation;

namespace ObrotoSklad.Application.Customers;

public class UpdateCustomerDtoValidator : AbstractValidator<UpgradeCustomerDto>
{
    public UpdateCustomerDtoValidator()
    {
        RuleFor(c => c.Name)
            .NotEmpty().WithMessage("Imię jest wymagane.")
            .MaximumLength(200).WithMessage("Imię nie może przekraczać 200 znaków.");

        RuleFor(c => c.Mail)
            .NotEmpty().WithMessage("Adres email jest wymagany.")
            .EmailAddress().WithMessage("Niepoprawny format emaila.");

        RuleFor(c => c.Adress)
            .NotEmpty().WithMessage("Adres jest wymagany.")
            .MaximumLength(300).WithMessage("Adres nie może przekraczać 200 znaków.");

        RuleFor(c => c.PhoneNumber)
            .Matches(@"^\d{9}$");
    }
}
