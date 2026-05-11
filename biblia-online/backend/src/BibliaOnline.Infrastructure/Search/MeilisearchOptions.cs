namespace BibliaOnline.Infrastructure.Search;

public sealed class MeilisearchOptions
{
    public const string SectionName = "Meilisearch";

    public string Url { get; set; } = "http://localhost:7700";
    public string ApiKey { get; set; } = string.Empty;
    public string VersesIndex { get; set; } = "verses";
    public bool SyncOnStartup { get; set; } = true;
}
