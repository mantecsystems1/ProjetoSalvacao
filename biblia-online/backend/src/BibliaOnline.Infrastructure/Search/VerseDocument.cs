using System.Text.Json.Serialization;

namespace BibliaOnline.Infrastructure.Search;

public sealed class VerseDocument
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;

    [JsonPropertyName("bibleVersionId")]
    public string BibleVersionId { get; set; } = string.Empty;

    [JsonPropertyName("versionCode")]
    public string VersionCode { get; set; } = string.Empty;

    [JsonPropertyName("languageCode")]
    public string LanguageCode { get; set; } = string.Empty;

    [JsonPropertyName("bookId")]
    public string BookId { get; set; } = string.Empty;

    [JsonPropertyName("bookSlug")]
    public string BookSlug { get; set; } = string.Empty;

    [JsonPropertyName("bookTitle")]
    public string BookTitle { get; set; } = string.Empty;

    [JsonPropertyName("chapterNumber")]
    public int ChapterNumber { get; set; }

    [JsonPropertyName("verseNumber")]
    public int VerseNumber { get; set; }

    [JsonPropertyName("text")]
    public string Text { get; set; } = string.Empty;
}
