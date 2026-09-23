using System;
using FluentValidation;

namespace ObrotoSklad.Application.Auth;

public class RegisterDtoValidator : AbstractValidator<RegisterDto>
{
    public RegisterDtoValidator()
    {
        RuleFor(r => r.Email)
            .NotEmpty()
            .EmailAddress();

        RuleFor(r => r.Password)
            .NotEmpty()
            .MinimumLength(8);

        RuleFor(r => r.FirstName)
            .NotEmpty();
        
        RuleFor(r => r.LastName)
            .NotEmpty();
    }

}
