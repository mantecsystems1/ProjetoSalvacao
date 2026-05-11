namespace Biblia.Application.Abstractions;

public interface IJwtTokenIssuer
{
    string CreateAccessToken(Guid userId, string email, string displayName, out DateTime expiresAtUtc);
}
