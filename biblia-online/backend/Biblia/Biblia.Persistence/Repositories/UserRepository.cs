using Biblia.Application.Abstractions.Repositories;
using Biblia.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Biblia.Persistence.Repositories;

public sealed class UserRepository(BibliaDbContext db) : IUserRepository
{
    public Task<User?> FindByEmailNormalizedAsync(string normalizedEmail, CancellationToken cancellationToken = default) =>
        db.Users.FirstOrDefaultAsync(u => u.Email == normalizedEmail, cancellationToken);

    public Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        db.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Id == id, cancellationToken);

    public async Task AddAsync(User user, CancellationToken cancellationToken = default)
    {
        await db.Users.AddAsync(user, cancellationToken);
    }
}
