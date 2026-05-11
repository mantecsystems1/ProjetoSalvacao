using Biblia.Domain.Entities;

namespace Biblia.Application.Abstractions.Repositories;

public interface IBookRepository
{
    Task<(IReadOnlyList<Book> Items, int TotalCount)> GetPagedAsync(int skip, int take, CancellationToken cancellationToken = default);
    Task<Book?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
}
