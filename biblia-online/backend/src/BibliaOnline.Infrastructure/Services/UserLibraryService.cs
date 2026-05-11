using BibliaOnline.Application.Abstractions;
using BibliaOnline.Application.Dtos;
using BibliaOnline.Domain.Entities;
using BibliaOnline.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace BibliaOnline.Infrastructure.Services;

public sealed class UserLibraryService(AppDbContext db) : IUserLibraryService
{
    public async Task<IReadOnlyList<FavoriteDto>> GetFavoritesAsync(Guid userId, CancellationToken ct = default)
    {
        return await (
            from f in db.FavoriteVerses.AsNoTracking()
            join v in db.BibleVersions.AsNoTracking() on f.BibleVersionId equals v.Id
            join b in db.Books.AsNoTracking() on f.BookId equals b.Id
            join t in db.BookTitles.AsNoTracking() on new { f.BibleVersionId, f.BookId } equals new { t.BibleVersionId, t.BookId }
            where f.UserId == userId
            orderby f.CreatedAtUtc descending
            select new FavoriteDto(
                f.Id,
                f.BibleVersionId,
                v.Code,
                f.BookId,
                b.Slug,
                t.Title,
                f.ChapterNumber,
                f.VerseNumber,
                f.CreatedAtUtc)
        ).ToListAsync(ct);
    }

    public async Task<FavoriteDto> AddFavoriteAsync(Guid userId, AddFavoriteRequest request, CancellationToken ct = default)
    {
        var exists = await db.FavoriteVerses.AnyAsync(f =>
            f.UserId == userId &&
            f.BibleVersionId == request.BibleVersionId &&
            f.BookId == request.BookId &&
            f.ChapterNumber == request.ChapterNumber &&
            f.VerseNumber == request.VerseNumber, ct);
        if (exists)
            throw new InvalidOperationException("Versículo já está nos favoritos.");

        var verseExists = await db.Verses.AnyAsync(v =>
            v.BibleVersionId == request.BibleVersionId &&
            v.BookId == request.BookId &&
            v.ChapterNumber == request.ChapterNumber &&
            v.VerseNumber == request.VerseNumber, ct);
        if (!verseExists)
            throw new InvalidOperationException("Versículo não encontrado para esta versão.");

        var fav = new FavoriteVerse
        {
            UserId = userId,
            BibleVersionId = request.BibleVersionId,
            BookId = request.BookId,
            ChapterNumber = request.ChapterNumber,
            VerseNumber = request.VerseNumber
        };
        db.FavoriteVerses.Add(fav);
        await db.SaveChangesAsync(ct);

        return await (
            from f in db.FavoriteVerses.AsNoTracking()
            join v in db.BibleVersions.AsNoTracking() on f.BibleVersionId equals v.Id
            join b in db.Books.AsNoTracking() on f.BookId equals b.Id
            join t in db.BookTitles.AsNoTracking() on new { f.BibleVersionId, f.BookId } equals new { t.BibleVersionId, t.BookId }
            where f.Id == fav.Id
            select new FavoriteDto(
                f.Id,
                f.BibleVersionId,
                v.Code,
                f.BookId,
                b.Slug,
                t.Title,
                f.ChapterNumber,
                f.VerseNumber,
                f.CreatedAtUtc)
        ).FirstAsync(ct);
    }

    public async Task RemoveFavoriteAsync(Guid userId, Guid favoriteId, CancellationToken ct = default)
    {
        var rows = await db.FavoriteVerses.Where(f => f.Id == favoriteId && f.UserId == userId).ExecuteDeleteAsync(ct);
        if (rows == 0)
            throw new KeyNotFoundException();
    }

    public async Task<IReadOnlyList<ReadingHistoryDto>> GetHistoryAsync(Guid userId, int take = 50, CancellationToken ct = default)
    {
        take = Math.Clamp(take, 1, 200);
        return await (
            from h in db.ReadingHistory.AsNoTracking()
            join v in db.BibleVersions.AsNoTracking() on h.BibleVersionId equals v.Id
            join b in db.Books.AsNoTracking() on h.BookId equals b.Id
            join t in db.BookTitles.AsNoTracking() on new { h.BibleVersionId, h.BookId } equals new { t.BibleVersionId, t.BookId }
            where h.UserId == userId
            orderby h.LastReadAtUtc descending
            select new ReadingHistoryDto(
                h.Id,
                h.BibleVersionId,
                v.Code,
                h.BookId,
                b.Slug,
                t.Title,
                h.ChapterNumber,
                h.LastReadAtUtc)
        ).Take(take).ToListAsync(ct);
    }

    public async Task TouchHistoryAsync(Guid userId, TouchHistoryRequest request, CancellationToken ct = default)
    {
        var chapterExists = await db.Verses.AnyAsync(v =>
            v.BibleVersionId == request.BibleVersionId &&
            v.BookId == request.BookId &&
            v.ChapterNumber == request.ChapterNumber, ct);
        if (!chapterExists)
            throw new InvalidOperationException("Capítulo não encontrado para esta versão.");

        var existing = await db.ReadingHistory.FirstOrDefaultAsync(h =>
            h.UserId == userId &&
            h.BibleVersionId == request.BibleVersionId &&
            h.BookId == request.BookId &&
            h.ChapterNumber == request.ChapterNumber, ct);

        var now = DateTime.UtcNow;
        if (existing is null)
        {
            db.ReadingHistory.Add(new ReadingHistoryEntry
            {
                UserId = userId,
                BibleVersionId = request.BibleVersionId,
                BookId = request.BookId,
                ChapterNumber = request.ChapterNumber,
                LastReadAtUtc = now
            });
        }
        else
            existing.LastReadAtUtc = now;

        await db.SaveChangesAsync(ct);
    }
}
