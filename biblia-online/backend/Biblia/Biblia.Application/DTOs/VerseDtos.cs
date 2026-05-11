namespace Biblia.Application.DTOs;

public sealed record VerseListItemDto(Guid Id, Guid ChapterId, Guid BibleVersionId, int Number, string Text);
