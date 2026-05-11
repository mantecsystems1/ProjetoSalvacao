using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Biblia.Application.Abstractions;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Biblia.Infrastructure.Security;

public sealed class JwtTokenIssuer(IOptions<JwtOptions> options) : IJwtTokenIssuer
{
    private readonly JwtOptions _opt = options.Value;

    public string CreateAccessToken(Guid userId, string email, string displayName, out DateTime expiresAtUtc)
    {
        if (string.IsNullOrWhiteSpace(_opt.SigningKey) || _opt.SigningKey.Length < 32)
            throw new InvalidOperationException("Jwt:SigningKey deve ter pelo menos 32 caracteres.");

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_opt.SigningKey));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        expiresAtUtc = DateTime.UtcNow.AddMinutes(_opt.AccessTokenMinutes);

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, userId.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, email),
            new Claim("display_name", displayName),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var token = new JwtSecurityToken(_opt.Issuer, _opt.Audience, claims, expires: expiresAtUtc, signingCredentials: creds);
        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
