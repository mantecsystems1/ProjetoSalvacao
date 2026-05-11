using Biblia.Application.DTOs;
using FluentValidation;

namespace Biblia.Application.Validation;

public sealed class CreateFavoriteRequestValidator : AbstractValidator<CreateFavoriteRequest>
{
    public CreateFavoriteRequestValidator()
    {
        RuleFor(x => x.VerseId).NotEmpty();
    }
}
