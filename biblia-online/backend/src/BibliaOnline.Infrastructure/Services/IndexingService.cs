using BibliaOnline.Application.Abstractions;
using BibliaOnline.Infrastructure.Persistence;
using BibliaOnline.Infrastructure.Search;
using Meilisearch;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace BibliaOnline.Infrastructure.Services;

public sealed class IndexingService(
    AppDbContext db,
    IOptions<MeilisearchOptions> options,
    ILogger<IndexingService> log) : IIndexingService
{
    private readonly MeilisearchOptions _opt = options.Value;

    public async Task EnsureSearchInfrastructureAsync(CancellationToken ct = default)
    {
        var client = new MeilisearchClient(_opt.Url, _opt.ApiKey);

        try
        {
            await client.CreateIndexAsync(_opt.VersesIndex, "id", ct);
        }
        catch (MeilisearchApiError ex) when (ex.Message.Contains("already exists", StringComparison.OrdinalIgnoreCase))
        {
            // índice já criado
        }
        catch (MeilisearchApiError ex) when (ex.Code == "index_already_exists")
        {
        }

        var index = client.Index(_opt.VersesIndex);
        var settings = new Settings
        {
            SearchableAttributes = new[] { "text", "bookTitle", "bookSlug", "versionCode" },
            FilterableAttributes = new[] { "bibleVersionId", "bookSlug", "languageCode", "versionCode" },
            SortableAttributes = new[] { "chapterNumber", "verseNumber" },
            RankingRules = new[] { "words", "typo", "proximity", "attribute", "sort", "exactness" }
        };
        await index.UpdateSettingsAsync(settings, ct);
    }

    public async Task ReindexAllVersesAsync(CancellationToken ct = default)
    {
        await EnsureSearchInfrastructureAsync(ct);

        var client = new MeilisearchClient(_opt.Url, _opt.ApiKey);
        var index = client.Index(_opt.VersesIndex);

        var verses = await db.Verses.AsNoTracking()
            .Include(v => v.BibleVersion).ThenInclude(bv => bv.Language)
            .Include(v => v.Book)
            .Select(v => new
            {
                v.Id,
                v.BibleVersionId,
                VersionCode = v.BibleVersion.Code,
                LanguageCode = v.BibleVersion.Language.Code,
                v.BookId,
                BookSlug = v.Book.Slug,
                BookTitle = v.BibleVersion.BookTitles.Where(t => t.BookId == v.BookId).Select(t => t.Title).FirstOrDefault() ?? v.Book.Slug,
                v.ChapterNumber,
                v.VerseNumber,
                v.Text
            })
            .ToListAsync(ct);

        var docs = verses.Select(v => new VerseDocument
        {
            Id = v.Id.ToString(),
            BibleVersionId = v.BibleVersionId.ToString(),
            VersionCode = v.VersionCode,
            LanguageCode = v.LanguageCode,
            BookId = v.BookId.ToString(),
            BookSlug = v.BookSlug,
            BookTitle = v.BookTitle,
            ChapterNumber = v.ChapterNumber,
            VerseNumber = v.VerseNumber,
            Text = v.Text
        }).ToList();

        await index.DeleteAllDocumentsAsync(ct);

        const int batch = 2000;
        for (var i = 0; i < docs.Count; i += batch)
        {
            var chunk = docs.Skip(i).Take(batch).ToList();
            var task = await index.AddDocumentsAsync(chunk, primaryKey: "id", cancellationToken: ct);
            await client.WaitForTaskAsync(task.TaskUid, timeoutMs: 120_000d, cancellationToken: ct);
        }

        log.LogInformation("Meilisearch: indexados {Count} versículos.", docs.Count);
    }
}
