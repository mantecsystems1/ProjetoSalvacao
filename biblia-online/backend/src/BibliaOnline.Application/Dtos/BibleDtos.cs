namespace BibliaOnline.Application.Dtos;

public record LanguageDto(Guid Id, string Code, string NativeName, string EnglishName, bool IsRtl);

public record BibleVersionDto(Guid Id, Guid LanguageId, string Code, string Name, string? Description, bool IsActive);

public record BookListItemDto(Guid Id, int CanonicalNumber, string Slug, string Title);

public record VerseDto(int VerseNumber, string Text);

public record ChapterDto(
    Guid BibleVersionId,
    string VersionCode,
    Guid BookId,
    string BookSlug,
    string BookTitle,
    int ChapterNumber,
    IReadOnlyList<VerseDto> Verses);

public record SearchHitDto(
    Guid BibleVersionId,
    string VersionCode,
    Guid BookId,
    string BookSlug,
    string BookTitle,
    int ChapterNumber,
    int VerseNumber,
    string Text,
    string? Formatted);

public record SearchResultDto(
    IReadOnlyList<SearchHitDto> Items,
    int Page,
    int PageSize,
    bool HasMore);
