using Biblia.Application.Common;
using Biblia.Application.DTOs;

namespace Biblia.Application.Abstractions.Services;

public interface IBookAppService
{
    Task<PagedResult<BookListItemDto>> ListAsync(PageRequest pageRequest, CancellationToken cancellationToken = default);
    Task<BookDetailDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
}
