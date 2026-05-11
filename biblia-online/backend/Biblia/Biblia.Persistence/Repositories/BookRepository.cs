using Biblia.Application.Abstractions.Repositories;
using Biblia.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Biblia.Persistence.Repositories;

public sealed class BookRepository(BibliaDbContext db) : IBookRepository
{
    public async Task<(IReadOnlyList<Book> Items, int TotalCount)> GetPagedAsync(int skip, int take, CancellationToken cancellationToken = default)
    {
        var query = db.Books.AsNoTracking().OrderBy(b => b.Order);
        var total = await query.CountAsync(cancellationToken);
        var items = await query.Skip(skip).Take(take).ToListAsync(cancellationToken);
        return (items, total);
    }

    public Task<Book?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        db.Books.AsNoTracking().FirstOrDefaultAsync(b => b.Id == id, cancellationToken);
}
