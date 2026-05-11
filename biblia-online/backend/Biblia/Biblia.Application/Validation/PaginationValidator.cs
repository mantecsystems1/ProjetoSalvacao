using Biblia.Application.Common;
using FluentValidation;

namespace Biblia.Application.Validation;

public sealed class PageRequestValidator : AbstractValidator<PageRequest>
{
    public PageRequestValidator()
    {
        RuleFor(x => x.Page).GreaterThanOrEqualTo(1).LessThanOrEqualTo(50_000);
        RuleFor(x => x.PageSize).InclusiveBetween(1, 100);
    }
}
