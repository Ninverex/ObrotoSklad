using System;
using FluentValidation;

namespace ObrotoSklad.Application.Auth;

public class LoginDtoValidator : AbstractValidator<LoginDto>
{
    public LoginDtoValidator()
    {
        RuleFor(r => r.Email)
            .NotEmpty()
            .EmailAddress();

        RuleFor(r => r.Password)
            .NotEmpty()
            .MinimumLength(8);
    }
}
