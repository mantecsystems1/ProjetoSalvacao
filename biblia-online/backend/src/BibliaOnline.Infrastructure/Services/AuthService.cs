using BibliaOnline.Application.Abstractions;
using BibliaOnline.Application.Dtos;
using BibliaOnline.Domain.Entities;
using BibliaOnline.Infrastructure.Persistence;
using BibliaOnline.Infrastructure.Security;
using Microsoft.EntityFrameworkCore;

namespace BibliaOnline.Infrastructure.Services;

public sealed class AuthService(AppDbContext db, IJwtTokenIssuer jwt) : IAuthService
{
    public async Task<AuthResponse> RegisterAsync(RegisterRequest request, CancellationToken ct = default)
    {
        var normalized = request.Email.Trim().ToLowerInvariant();
        if (await db.Users.AnyAsync(u => u.Email == normalized, ct))
            throw new InvalidOperationException("Email já cadastrado.");

        var user = new UserAccount
        {
            Email = normalized,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
            DisplayName = request.DisplayName.Trim()
        };
        db.Users.Add(user);
        await db.SaveChangesAsync(ct);

        var token = jwt.CreateAccessToken(user.Id, user.Email, user.DisplayName, out var exp);
        return new AuthResponse(token, exp, new UserDto(user.Id, user.Email, user.DisplayName));
    }

    public async Task<AuthResponse?> LoginAsync(LoginRequest request, CancellationToken ct = default)
    {
        var normalized = request.Email.Trim().ToLowerInvariant();
        var user = await db.Users.FirstOrDefaultAsync(u => u.Email == normalized, ct);
        if (user is null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            return null;

        var token = jwt.CreateAccessToken(user.Id, user.Email, user.DisplayName, out var exp);
        return new AuthResponse(token, exp, new UserDto(user.Id, user.Email, user.DisplayName));
    }
}
