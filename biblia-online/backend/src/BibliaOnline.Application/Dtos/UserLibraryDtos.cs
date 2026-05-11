namespace BibliaOnline.Application.Dtos;

public record FavoriteDto(
    Guid Id,
    Guid BibleVersionId,
    string VersionCode,
    Guid BookId,
    string BookSlug,
    string BookTitle,
    int ChapterNumber,
    int VerseNumber,
    DateTime CreatedAtUtc);

public record AddFavoriteRequest(Guid BibleVersionId, Guid BookId, int ChapterNumber, int VerseNumber);

public record ReadingHistoryDto(
    Guid Id,
    Guid BibleVersionId,
    string VersionCode,
    Guid BookId,
    string BookSlug,
    string BookTitle,
    int ChapterNumber,
    DateTime LastReadAtUtc);

public record TouchHistoryRequest(Guid BibleVersionId, Guid BookId, int ChapterNumber);
