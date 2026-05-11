using Biblia.Domain.Entities;

namespace Biblia.Application.Abstractions.Repositories;

public interface IFavoriteVerseRepository
{
    Task<(IReadOnlyList<FavoriteVerse> Items, int TotalCount)> GetByUserPagedAsync(Guid userId, int skip, int take, CancellationToken cancellationToken = default);
    Task<bool> ExistsAsync(Guid userId, Guid verseId, CancellationToken cancellationToken = default);
    Task AddAsync(FavoriteVerse favorite, CancellationToken cancellationToken = default);
}
