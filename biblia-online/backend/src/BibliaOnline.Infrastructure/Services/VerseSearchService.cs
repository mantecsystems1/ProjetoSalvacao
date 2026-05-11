using BibliaOnline.Application.Abstractions;
using BibliaOnline.Application.Dtos;
using BibliaOnline.Infrastructure.Search;
using Meilisearch;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace BibliaOnline.Infrastructure.Services;

public sealed class VerseSearchService(IOptions<MeilisearchOptions> options, ILogger<VerseSearchService> log) : IVerseSearchService
{
    private readonly MeilisearchOptions _opt = options.Value;

    public async Task<IReadOnlyList<SearchHitDto>> SearchAsync(string query, Guid? bibleVersionId, int limit, CancellationToken ct = default)
    {
        limit = Math.Clamp(limit, 1, 100);
        var q = (query ?? string.Empty).Trim();
        if (q.Length < 2)
            return Array.Empty<SearchHitDto>();

        try
        {
            var client = new MeilisearchClient(_opt.Url, _opt.ApiKey);
            var index = client.Index(_opt.VersesIndex);

            var sq = new SearchQuery
            {
                Limit = limit,
                AttributesToHighlight = new[] { "text" }
            };
            if (bibleVersionId.HasValue)
                sq.Filter = $"bibleVersionId = \"{bibleVersionId.Value}\"";

            var result = await index.SearchAsync<VerseDocument>(q, sq, ct);
            var hits = result.Hits ?? Array.Empty<VerseDocument>();
            var list = new List<SearchHitDto>(hits.Count);

            foreach (var h in hits)
            {
                if (!Guid.TryParse(h.BibleVersionId, out var vid) ||
                    !Guid.TryParse(h.BookId, out var bid))
                    continue;

                list.Add(new SearchHitDto(
                    vid,
                    h.VersionCode,
                    bid,
                    h.BookSlug,
                    h.BookTitle,
                    h.ChapterNumber,
                    h.VerseNumber,
                    h.Text,
                    Formatted: null));
            }

            return list;
        }
        catch (Exception ex)
        {
            log.LogWarning(ex, "Meilisearch indisponível; retornando lista vazia.");
            return Array.Empty<SearchHitDto>();
        }
    }
}
