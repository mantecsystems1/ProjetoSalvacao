using BibliaOnline.Application.Abstractions;
using BibliaOnline.Application.Dtos;
using BibliaOnline.Infrastructure.Search;
using Meilisearch;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Text.RegularExpressions;

namespace BibliaOnline.Infrastructure.Services;

public sealed class VerseSearchService(IOptions<MeilisearchOptions> options, ILogger<VerseSearchService> log) : IVerseSearchService
{
    private readonly MeilisearchOptions _opt = options.Value;

    public async Task<SearchResultDto> SearchAsync(string query, Guid? bibleVersionId, int page, int pageSize, CancellationToken ct = default)
    {
        page = Math.Max(1, page);
        pageSize = Math.Clamp(pageSize, 1, 100);

        var q = (query ?? string.Empty).Trim();
        if (q.Length < 2)
            return new SearchResultDto(Array.Empty<SearchHitDto>(), page, pageSize, HasMore: false);

        var (searchText, chapterFilter, verseFilter) = ParseReferenceQuery(q);

        try
        {
            var client = new MeilisearchClient(_opt.Url, _opt.ApiKey);
            var index = client.Index(_opt.VersesIndex);

            var sq = new SearchQuery
            {
                Limit = pageSize,
                Offset = (page - 1) * pageSize,
                AttributesToHighlight = new[] { "text" },
                HighlightPreTag = "<mark>",
                HighlightPostTag = "</mark>"
            };

            var filters = new List<string>();
            if (bibleVersionId.HasValue)
                filters.Add($"bibleVersionId = \"{bibleVersionId.Value}\"");

            if (chapterFilter.HasValue)
                filters.Add($"chapterNumber = {chapterFilter.Value}");

            if (verseFilter.HasValue)
                filters.Add($"verseNumber = {verseFilter.Value}");

            if (filters.Count > 0)
                sq.Filter = string.Join(" AND ", filters);

            var result = await index.SearchAsync<VerseDocument>(searchText, sq, ct);
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

            return new SearchResultDto(list, page, pageSize, hits.Count == pageSize);
        }
        catch (Exception ex)
        {
            log.LogWarning(ex, "Meilisearch indisponível; retornando resultados vazios.");
            return new SearchResultDto(Array.Empty<SearchHitDto>(), page, pageSize, HasMore: false);
        }
    }

    private static (string Query, int? Chapter, int? Verse) ParseReferenceQuery(string query)
    {
        var trimmed = query.Trim();
        var match = Regex.Match(trimmed, @"^(.+?)\s+(\d+)(?::|\s+)?(\d+)?$", RegexOptions.Compiled | RegexOptions.CultureInvariant);
        if (!match.Success)
            return (trimmed, null, null);

        var bookPart = match.Groups[1].Value.Trim();
        var chapter = int.Parse(match.Groups[2].Value);
        var verse = match.Groups[3].Success ? int.Parse(match.Groups[3].Value) as int? : null;

        return (string.IsNullOrWhiteSpace(bookPart) ? trimmed : bookPart, chapter, verse);
    }
}
