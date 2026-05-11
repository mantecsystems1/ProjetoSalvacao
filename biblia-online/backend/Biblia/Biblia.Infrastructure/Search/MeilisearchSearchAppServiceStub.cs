using Biblia.Application.Abstractions.Services;
using Biblia.Application.Common;
using Biblia.Application.DTOs;
using Microsoft.Extensions.Logging;

namespace Biblia.Infrastructure.Search;

/// <summary>Placeholder para futura integração Meilisearch (substitua registro no DI quando implementado).</summary>
public sealed class MeilisearchSearchAppServiceStub(ILogger<MeilisearchSearchAppServiceStub> log) : ISearchAppService
{
    public Task<PagedResult<VerseSearchResultDto>> SearchAsync(string query, Guid? bibleVersionId, PageRequest pageRequest, CancellationToken cancellationToken = default)
    {
        log.LogWarning("Busca Meilisearch não configurada; retornando vazio. Utilize Search:Provider=Sql.");
        return Task.FromResult(new PagedResult<VerseSearchResultDto>
        {
            Items = Array.Empty<VerseSearchResultDto>(),
            Page = pageRequest.Page,
            PageSize = pageRequest.PageSize,
            TotalCount = 0
        });
    }
}
