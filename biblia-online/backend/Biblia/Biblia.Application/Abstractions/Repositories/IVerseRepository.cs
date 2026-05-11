using Biblia.Domain.Entities;

namespace Biblia.Application.Abstractions.Repositories;

public interface IVerseRepository
{
    Task<IReadOnlyList<Verse>> GetByChapterAndVersionAsync(Guid chapterId, Guid bibleVersionId, CancellationToken cancellationToken = default);
    Task<Verse?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<(IReadOnlyList<Verse> Items, int TotalCount)> SearchAsync(string query, Guid? bibleVersionId, int skip, int take, CancellationToken cancellationToken = default);
}
