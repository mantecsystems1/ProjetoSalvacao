using BibliaOnline.Application.Dtos;

namespace BibliaOnline.Application.Abstractions;

public interface IVerseSearchService
{
    Task<IReadOnlyList<SearchHitDto>> SearchAsync(string query, Guid? bibleVersionId, int limit, CancellationToken ct = default);
}
