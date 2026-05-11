using Biblia.Application.DTOs;

namespace Biblia.Application.Abstractions.Services;

public interface IChapterAppService
{
    Task<IReadOnlyList<ChapterListItemDto>> ListByBookAsync(Guid bookId, CancellationToken cancellationToken = default);
}
