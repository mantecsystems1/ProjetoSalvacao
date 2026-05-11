using BibliaOnline.Application.Dtos;
using FluentValidation;

namespace BibliaOnline.Application.Validation;

public class RegisterRequestValidator : AbstractValidator<RegisterRequest>
{
    public RegisterRequestValidator()
    {
        RuleFor(x => x.Email).NotEmpty().EmailAddress().MaximumLength(320);
        RuleFor(x => x.Password).MinimumLength(8).MaximumLength(256);
        RuleFor(x => x.DisplayName).NotEmpty().MaximumLength(120);
    }
}
