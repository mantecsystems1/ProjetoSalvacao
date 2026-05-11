using Biblia.Application.Common;
using Biblia.Application.DTOs;

namespace Biblia.Application.Abstractions.Services;

public interface ISearchAppService
{
    Task<PagedResult<VerseSearchResultDto>> SearchAsync(string query, Guid? bibleVersionId, PageRequest pageRequest, CancellationToken cancellationToken = default);
}
