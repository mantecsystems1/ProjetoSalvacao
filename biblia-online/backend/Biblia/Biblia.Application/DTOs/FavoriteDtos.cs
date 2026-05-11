namespace Biblia.Application.DTOs;

public sealed record FavoriteListItemDto(Guid Id, Guid VerseId, DateTime CreatedAtUtc);

public sealed record CreateFavoriteRequest(Guid VerseId);
