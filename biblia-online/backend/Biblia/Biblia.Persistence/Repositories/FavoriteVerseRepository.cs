using Biblia.Application.Abstractions.Repositories;
using Biblia.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Biblia.Persistence.Repositories;

public sealed class FavoriteVerseRepository(BibliaDbContext db) : IFavoriteVerseRepository
{
    public async Task<(IReadOnlyList<FavoriteVerse> Items, int TotalCount)> GetByUserPagedAsync(Guid userId, int skip, int take, CancellationToken cancellationToken = default)
    {
        var query = db.FavoriteVerses.AsNoTracking().Where(f => f.UserId == userId).OrderByDescending(f => f.CreatedAtUtc);
        var total = await query.CountAsync(cancellationToken);
        var items = await query.Skip(skip).Take(take).ToListAsync(cancellationToken);
        return (items, total);
    }

    public Task<bool> ExistsAsync(Guid userId, Guid verseId, CancellationToken cancellationToken = default) =>
        db.FavoriteVerses.AnyAsync(f => f.UserId == userId && f.VerseId == verseId, cancellationToken);

    public async Task AddAsync(FavoriteVerse favorite, CancellationToken cancellationToken = default)
    {
        await db.FavoriteVerses.AddAsync(favorite, cancellationToken);
    }
}
