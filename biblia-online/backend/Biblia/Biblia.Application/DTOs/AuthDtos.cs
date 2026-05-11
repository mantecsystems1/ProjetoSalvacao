namespace Biblia.Application.DTOs;

public sealed record RegisterRequest(string Email, string Password, string DisplayName);

public sealed record LoginRequest(string Email, string Password);

public sealed record AuthResponseDto(string AccessToken, DateTime ExpiresAtUtc, UserSummaryDto User);

public sealed record UserSummaryDto(Guid Id, string Email, string DisplayName);
