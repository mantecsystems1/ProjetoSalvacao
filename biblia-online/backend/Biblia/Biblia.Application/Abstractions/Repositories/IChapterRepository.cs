using Biblia.Domain.Entities;

namespace Biblia.Application.Abstractions.Repositories;

public interface IChapterRepository
{
    Task<IReadOnlyList<Chapter>> GetByBookIdOrderedAsync(Guid bookId, CancellationToken cancellationToken = default);
    Task<Chapter?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
}
