using Biblia.Application.Abstractions.Repositories;
using Biblia.Application.Abstractions.Services;
using Biblia.Application.Common;
using Biblia.Application.DTOs;
using Biblia.Domain.Entities;
using FluentValidation;
using FluentValidation.Results;

namespace Biblia.Persistence.Services;

public sealed class SqlSearchAppService(IVerseRepository verses) : ISearchAppService
{
    public async Task<PagedResult<VerseSearchResultDto>> SearchAsync(string query, Guid? bibleVersionId, PageRequest pageRequest, CancellationToken cancellationToken = default)
    {
        var q = (query ?? string.Empty).Trim();
        if (q.Length < 2)
            throw new ValidationException(new[] { new ValidationFailure("q", "Informe pelo menos 2 caracteres.") });

        var (items, total) = await verses.SearchAsync(q, bibleVersionId, pageRequest.Skip, pageRequest.PageSize, cancellationToken);
        var dto = items.Select(Map).ToList();
        return new PagedResult<VerseSearchResultDto>
        {
            Items = dto,
            Page = pageRequest.Page,
            PageSize = pageRequest.PageSize,
            TotalCount = total
        };
    }

    private static VerseSearchResultDto Map(Verse v) =>
        new(
            v.Id,
            v.ChapterId,
            v.Chapter!.BookId,
            v.Chapter.Book!.Slug,
            v.Chapter.Book.Name,
            v.Chapter.Number,
            v.BibleVersionId,
            v.BibleVersion!.Code,
            v.Number,
            v.Text);
}
