namespace BibliaOnline.Infrastructure.Security;

public interface IJwtTokenIssuer
{
    string CreateAccessToken(Guid userId, string email, string displayName, out DateTime expiresAtUtc);
}
