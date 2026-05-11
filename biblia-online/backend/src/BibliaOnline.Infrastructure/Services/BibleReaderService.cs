using BibliaOnline.Application.Abstractions;
using BibliaOnline.Application.Dtos;
using BibliaOnline.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace BibliaOnline.Infrastructure.Services;

public sealed class BibleReaderService(AppDbContext db) : IBibleReaderService
{
    public async Task<IReadOnlyList<LanguageDto>> GetLanguagesAsync(CancellationToken ct = default)
    {
        return await db.Languages.AsNoTracking()
            .OrderBy(x => x.Code)
            .Select(x => new LanguageDto(x.Id, x.Code, x.NativeName, x.EnglishName, x.IsRtl))
            .ToListAsync(ct);
    }

    public async Task<IReadOnlyList<BibleVersionDto>> GetVersionsAsync(Guid? languageId, CancellationToken ct = default)
    {
        var q = db.BibleVersions.AsNoTracking().Where(x => x.IsActive);
        if (languageId.HasValue)
            q = q.Where(x => x.LanguageId == languageId.Value);
        return await q.OrderBy(x => x.Language.Code).ThenBy(x => x.Name)
            .Select(x => new BibleVersionDto(x.Id, x.LanguageId, x.Code, x.Name, x.Description, x.IsActive))
            .ToListAsync(ct);
    }

    public async Task<BibleVersionDto?> GetVersionByCodeAsync(string code, CancellationToken ct = default)
    {
        var normalized = code.Trim().ToLowerInvariant();
        return await db.BibleVersions.AsNoTracking()
            .Where(x => x.IsActive && x.Code == normalized)
            .Select(x => new BibleVersionDto(x.Id, x.LanguageId, x.Code, x.Name, x.Description, x.IsActive))
            .FirstOrDefaultAsync(ct);
    }

    public async Task<IReadOnlyList<BookListItemDto>> GetBooksAsync(Guid bibleVersionId, CancellationToken ct = default)
    {
        return await db.BookTitles.AsNoTracking()
            .Where(t => t.BibleVersionId == bibleVersionId)
            .OrderBy(t => t.Book.CanonicalNumber)
            .Select(t => new BookListItemDto(t.Book.Id, t.Book.CanonicalNumber, t.Book.Slug, t.Title))
            .ToListAsync(ct);
    }

    public async Task<ChapterDto?> GetChapterAsync(Guid bibleVersionId, string bookSlug, int chapterNumber, CancellationToken ct = default)
    {
        var version = await db.BibleVersions.AsNoTracking()
            .Include(v => v.Language)
            .FirstOrDefaultAsync(v => v.Id == bibleVersionId, ct);
        if (version is null)
            return null;

        var book = await db.Books.AsNoTracking().FirstOrDefaultAsync(b => b.Slug == bookSlug, ct);
        if (book is null)
            return null;

        var title = await db.BookTitles.AsNoTracking()
            .Where(t => t.BibleVersionId == bibleVersionId && t.BookId == book.Id)
            .Select(t => t.Title)
            .FirstOrDefaultAsync(ct);
        if (title is null)
            return null;

        var verses = await db.Verses.AsNoTracking()
            .Where(v => v.BibleVersionId == bibleVersionId && v.BookId == book.Id && v.ChapterNumber == chapterNumber)
            .OrderBy(v => v.VerseNumber)
            .Select(v => new VerseDto(v.VerseNumber, v.Text))
            .ToListAsync(ct);

        if (verses.Count == 0)
            return null;

        return new ChapterDto(
            bibleVersionId,
            version.Code,
            book.Id,
            book.Slug,
            title,
            chapterNumber,
            verses);
    }
}
