using Biblia.Application.DTOs;

namespace Biblia.Application.Abstractions.Services;

public interface IAuthAppService
{
    Task<AuthResponseDto> RegisterAsync(RegisterRequest request, CancellationToken cancellationToken = default);
    Task<AuthResponseDto?> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default);
}
