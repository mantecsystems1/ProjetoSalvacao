using Biblia.Application.Abstractions;
using Biblia.Application.Abstractions.Repositories;
using Biblia.Application.Abstractions.Services;
using Biblia.Application.Common;
using Biblia.Application.DTOs;
using Biblia.Domain.Entities;

namespace Biblia.Application.Services;

public sealed class FavoriteAppService(
    IFavoriteVerseRepository favorites,
    IVerseRepository verses,
    IUnitOfWork unitOfWork) : IFavoriteAppService
{
    public async Task<PagedResult<FavoriteListItemDto>> ListAsync(Guid userId, PageRequest pageRequest, CancellationToken cancellationToken = default)
    {
        var (items, total) = await favorites.GetByUserPagedAsync(userId, pageRequest.Skip, pageRequest.PageSize, cancellationToken);
        var dto = items.Select(f => new FavoriteListItemDto(f.Id, f.VerseId, f.CreatedAtUtc)).ToList();
        return new PagedResult<FavoriteListItemDto>
        {
            Items = dto,
            Page = pageRequest.Page,
            PageSize = pageRequest.PageSize,
            TotalCount = total
        };
    }

    public async Task AddAsync(Guid userId, CreateFavoriteRequest request, CancellationToken cancellationToken = default)
    {
        if (await verses.GetByIdAsync(request.VerseId, cancellationToken) is null)
            throw new KeyNotFoundException("Versículo não encontrado.");

        if (await favorites.ExistsAsync(userId, request.VerseId, cancellationToken))
            throw new InvalidOperationException("Versículo já favoritado.");

        await favorites.AddAsync(new FavoriteVerse
        {
            UserId = userId,
            VerseId = request.VerseId
        }, cancellationToken);

        await unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
