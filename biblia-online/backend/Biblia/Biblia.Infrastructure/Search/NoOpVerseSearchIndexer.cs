using Biblia.Application.Abstractions.Search;

namespace Biblia.Infrastructure.Search;

public sealed class NoOpVerseSearchIndexer : IVerseSearchIndexer
{
    public Task EnsureInfrastructureAsync(CancellationToken cancellationToken = default) => Task.CompletedTask;

    public Task ReindexAllAsync(CancellationToken cancellationToken = default) => Task.CompletedTask;
}
