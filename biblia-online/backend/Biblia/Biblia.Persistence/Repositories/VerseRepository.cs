using Biblia.Application.Abstractions.Repositories;
using Biblia.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Biblia.Persistence.Repositories;

public sealed class VerseRepository(BibliaDbContext db) : IVerseRepository
{
    public async Task<IReadOnlyList<Verse>> GetByChapterAndVersionAsync(Guid chapterId, Guid bibleVersionId, CancellationToken cancellationToken = default)
    {
        return await db.Verses.AsNoTracking()
            .Where(v => v.ChapterId == chapterId && v.BibleVersionId == bibleVersionId)
            .OrderBy(v => v.Number)
            .ToListAsync(cancellationToken);
    }

    public Task<Verse?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        db.Verses.AsNoTracking().FirstOrDefaultAsync(v => v.Id == id, cancellationToken);

    public async Task<(IReadOnlyList<Verse> Items, int TotalCount)> SearchAsync(string query, Guid? bibleVersionId, int skip, int take, CancellationToken cancellationToken = default)
    {
        var pattern = "%" + EscapeLike(query) + "%";

        var baseQuery = db.Verses.AsNoTracking()
            .Include(v => v.Chapter).ThenInclude(c => c.Book)
            .Include(v => v.BibleVersion)
            .Where(v => EF.Functions.ILike(v.Text, pattern));

        if (bibleVersionId.HasValue)
            baseQuery = baseQuery.Where(v => v.BibleVersionId == bibleVersionId.Value);

        var ordered = baseQuery
            .OrderBy(v => v.BibleVersion!.Code)
            .ThenBy(v => v.Chapter!.Book.Order)
            .ThenBy(v => v.Chapter!.Number)
            .ThenBy(v => v.Number);

        var total = await ordered.CountAsync(cancellationToken);
        var items = await ordered.Skip(skip).Take(take).ToListAsync(cancellationToken);
        return (items, total);
    }

    private static string EscapeLike(string value)
    {
        return value.Replace("\\", "\\\\", StringComparison.Ordinal).Replace("%", "\\%", StringComparison.Ordinal).Replace("_", "\\_", StringComparison.Ordinal);
    }
}
