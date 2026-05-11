using BibliaOnline.Application.Dtos;

namespace BibliaOnline.Application.Abstractions;

public interface IBibleReaderService
{
    Task<IReadOnlyList<LanguageDto>> GetLanguagesAsync(CancellationToken ct = default);
    Task<IReadOnlyList<BibleVersionDto>> GetVersionsAsync(Guid? languageId, CancellationToken ct = default);
    Task<BibleVersionDto?> GetVersionByCodeAsync(string code, CancellationToken ct = default);
    Task<IReadOnlyList<BookListItemDto>> GetBooksAsync(Guid bibleVersionId, CancellationToken ct = default);
    Task<ChapterDto?> GetChapterAsync(Guid bibleVersionId, string bookSlug, int chapterNumber, CancellationToken ct = default);
}
