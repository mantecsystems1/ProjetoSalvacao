using Biblia.Domain.Entities;

namespace Biblia.Application.Abstractions.Repositories;

public interface IUserRepository
{
    Task<User?> FindByEmailNormalizedAsync(string normalizedEmail, CancellationToken cancellationToken = default);
    Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task AddAsync(User user, CancellationToken cancellationToken = default);
}
