using BibliaOnline.Application.Dtos;

namespace BibliaOnline.Application.Abstractions;

public interface IUserLibraryService
{
    Task<IReadOnlyList<FavoriteDto>> GetFavoritesAsync(Guid userId, CancellationToken ct = default);
    Task<FavoriteDto> AddFavoriteAsync(Guid userId, AddFavoriteRequest request, CancellationToken ct = default);
    Task RemoveFavoriteAsync(Guid userId, Guid favoriteId, CancellationToken ct = default);

    Task<IReadOnlyList<ReadingHistoryDto>> GetHistoryAsync(Guid userId, int take = 50, CancellationToken ct = default);
    Task TouchHistoryAsync(Guid userId, TouchHistoryRequest request, CancellationToken ct = default);
}
