using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using BibliaOnline.Application.Abstractions;

namespace BibliaOnline.Api.Services;

public sealed class HttpCurrentUser(IHttpContextAccessor http) : ICurrentUser
{
    public Guid? UserId
    {
        get
        {
            var sub = http.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier)
                      ?? http.HttpContext?.User.FindFirstValue(JwtRegisteredClaimNames.Sub);
            return Guid.TryParse(sub, out var id) ? id : null;
        }
    }
}
