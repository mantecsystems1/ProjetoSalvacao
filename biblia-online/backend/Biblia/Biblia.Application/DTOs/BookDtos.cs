namespace Biblia.Application.DTOs;

public sealed record BookListItemDto(Guid Id, int Order, string Slug, string Name);

public sealed record BookDetailDto(Guid Id, int Order, string Slug, string Name);
