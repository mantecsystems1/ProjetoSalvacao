namespace Biblia.Application.DTOs;

public sealed record VerseSearchResultDto(
    Guid VerseId,
    Guid ChapterId,
    Guid BookId,
    string BookSlug,
    string BookName,
    int ChapterNumber,
    Guid BibleVersionId,
    string BibleVersionCode,
    int VerseNumber,
    string Text);
