using Biblia.Application.DTOs;

namespace Biblia.Application.Abstractions.Services;

public interface IVerseQueryAppService
{
    Task<IReadOnlyList<VerseListItemDto>> ListByChapterAsync(Guid chapterId, Guid bibleVersionId, CancellationToken cancellationToken = default);
}
