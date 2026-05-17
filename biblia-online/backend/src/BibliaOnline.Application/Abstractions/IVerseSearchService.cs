using BibliaOnline.Application.Dtos;

namespace BibliaOnline.Application.Abstractions;

public interface IVerseSearchService
{
    Task<SearchResultDto> SearchAsync(string query, Guid? bibleVersionId, int page, int pageSize, CancellationToken ct = default);
}
