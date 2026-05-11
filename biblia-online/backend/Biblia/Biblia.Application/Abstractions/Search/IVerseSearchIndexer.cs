namespace Biblia.Application.Abstractions.Search;

/// <summary>Ponto de extensão futuro para indexação Meilisearch (implementação real ficará na Infrastructure).</summary>
public interface IVerseSearchIndexer
{
    Task EnsureInfrastructureAsync(CancellationToken cancellationToken = default);

    Task ReindexAllAsync(CancellationToken cancellationToken = default);
}
