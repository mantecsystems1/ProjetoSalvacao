namespace Biblia.Application.Abstractions.Search;

public enum SearchProviderKind
{
    Sql = 0,
    Meilisearch = 1
}

public sealed class SearchOptions
{
    public const string SectionName = "Search";

    public SearchProviderKind Provider { get; set; } = SearchProviderKind.Sql;
}
