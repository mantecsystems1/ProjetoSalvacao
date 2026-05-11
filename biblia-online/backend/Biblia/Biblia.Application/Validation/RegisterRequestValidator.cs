using Biblia.Application.DTOs;
using FluentValidation;

namespace Biblia.Application.Validation;

public sealed class RegisterRequestValidator : AbstractValidator<RegisterRequest>
{
    public RegisterRequestValidator()
    {
        RuleFor(x => x.Email).NotEmpty().EmailAddress().MaximumLength(320);
        RuleFor(x => x.Password).MinimumLength(8).MaximumLength(256);
        RuleFor(x => x.DisplayName).NotEmpty().MaximumLength(120);
    }
}
