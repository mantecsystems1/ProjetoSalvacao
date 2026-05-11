using Biblia.Application.Common;
using Biblia.Application.DTOs;

namespace Biblia.Application.Abstractions.Services;

public interface IFavoriteAppService
{
    Task<PagedResult<FavoriteListItemDto>> ListAsync(Guid userId, PageRequest pageRequest, CancellationToken cancellationToken = default);
    Task AddAsync(Guid userId, CreateFavoriteRequest request, CancellationToken cancellationToken = default);
}
