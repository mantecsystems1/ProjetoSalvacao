using Biblia.Application.Abstractions;
using Biblia.Application.Abstractions.Repositories;
using Biblia.Application.Abstractions.Services;
using Biblia.Application.Common;
using Biblia.Application.DTOs;

namespace Biblia.Application.Services;

public sealed class BookAppService(IBookRepository books, ICacheStore cache) : IBookAppService
{
    private static readonly TimeSpan CacheTtl = TimeSpan.FromMinutes(2);

    public async Task<PagedResult<BookListItemDto>> ListAsync(PageRequest pageRequest, CancellationToken cancellationToken = default)
    {
        var cacheKey = $"books:list:{pageRequest.Page}:{pageRequest.PageSize}";
        var cached = await cache.GetAsync<PagedResult<BookListItemDto>>(cacheKey, cancellationToken);
        if (cached is not null)
            return cached;

        var (items, total) = await books.GetPagedAsync(pageRequest.Skip, pageRequest.PageSize, cancellationToken);
        var dto = items.Select(b => new BookListItemDto(b.Id, b.Order, b.Slug, b.Name)).ToList();
        var result = new PagedResult<BookListItemDto>
        {
            Items = dto,
            Page = pageRequest.Page,
            PageSize = pageRequest.PageSize,
            TotalCount = total
        };

        await cache.SetAsync(cacheKey, result, CacheTtl, cancellationToken);
        return result;
    }

    public async Task<BookDetailDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var cacheKey = $"books:detail:{id}";
        var cached = await cache.GetAsync<BookDetailDto>(cacheKey, cancellationToken);
        if (cached is not null)
            return cached;

        var book = await books.GetByIdAsync(id, cancellationToken);
        if (book is null)
            return null;

        var dto = new BookDetailDto(book.Id, book.Order, book.Slug, book.Name);
        await cache.SetAsync(cacheKey, dto, CacheTtl, cancellationToken);
        return dto;
    }
}
