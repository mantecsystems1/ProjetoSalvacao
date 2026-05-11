using Biblia.Application.Abstractions.Repositories;
using Biblia.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Biblia.Persistence.Repositories;

public sealed class ChapterRepository(BibliaDbContext db) : IChapterRepository
{
    public async Task<IReadOnlyList<Chapter>> GetByBookIdOrderedAsync(Guid bookId, CancellationToken cancellationToken = default)
    {
        return await db.Chapters.AsNoTracking()
            .Where(c => c.BookId == bookId)
            .OrderBy(c => c.Number)
            .ToListAsync(cancellationToken);
    }

    public Task<Chapter?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        db.Chapters.AsNoTracking().FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
}
