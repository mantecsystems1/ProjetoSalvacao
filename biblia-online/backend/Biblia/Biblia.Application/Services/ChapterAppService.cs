using Biblia.Application.Abstractions;
using Biblia.Application.Abstractions.Repositories;
using Biblia.Application.Abstractions.Services;
using Biblia.Application.DTOs;

namespace Biblia.Application.Services;

public sealed class ChapterAppService(IChapterRepository chapters, IBookRepository books, ICacheStore cache) : IChapterAppService
{
    private static readonly TimeSpan CacheTtl = TimeSpan.FromMinutes(2);

    public async Task<IReadOnlyList<ChapterListItemDto>> ListByBookAsync(Guid bookId, CancellationToken cancellationToken = default)
    {
        var cacheKey = $"chapters:book:{bookId}";
        var cached = await cache.GetAsync<List<ChapterListItemDto>>(cacheKey, cancellationToken);
        if (cached is not null)
            return cached;

        if (await books.GetByIdAsync(bookId, cancellationToken) is null)
            throw new KeyNotFoundException("Livro não encontrado.");

        var list = await chapters.GetByBookIdOrderedAsync(bookId, cancellationToken);
        var dto = list.Select(c => new ChapterListItemDto(c.Id, c.BookId, c.Number)).ToList();
        await cache.SetAsync(cacheKey, dto, CacheTtl, cancellationToken);
        return dto;
    }
}
