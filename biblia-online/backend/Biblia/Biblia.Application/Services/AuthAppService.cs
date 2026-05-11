using Biblia.Application.Abstractions;
using Biblia.Application.Abstractions.Repositories;
using Biblia.Application.Abstractions.Services;
using Biblia.Application.DTOs;
using Biblia.Domain.Entities;

namespace Biblia.Application.Services;

public sealed class AuthAppService(
    IUserRepository users,
    IPasswordHasher passwordHasher,
    IJwtTokenIssuer jwt,
    IUnitOfWork unitOfWork) : IAuthAppService
{
    public async Task<AuthResponseDto> RegisterAsync(RegisterRequest request, CancellationToken cancellationToken = default)
    {
        var normalized = request.Email.Trim().ToLowerInvariant();
        if (await users.FindByEmailNormalizedAsync(normalized, cancellationToken) is not null)
            throw new InvalidOperationException("Email já cadastrado.");

        var user = new User
        {
            Email = normalized,
            PasswordHash = passwordHasher.Hash(request.Password),
            DisplayName = request.DisplayName.Trim()
        };

        await users.AddAsync(user, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        var token = jwt.CreateAccessToken(user.Id, user.Email, user.DisplayName, out var exp);
        return new AuthResponseDto(token, exp, new UserSummaryDto(user.Id, user.Email, user.DisplayName));
    }

    public async Task<AuthResponseDto?> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default)
    {
        var normalized = request.Email.Trim().ToLowerInvariant();
        var user = await users.FindByEmailNormalizedAsync(normalized, cancellationToken);
        if (user is null || !passwordHasher.Verify(request.Password, user.PasswordHash))
            return null;

        var token = jwt.CreateAccessToken(user.Id, user.Email, user.DisplayName, out var exp);
        return new AuthResponseDto(token, exp, new UserSummaryDto(user.Id, user.Email, user.DisplayName));
    }
}
