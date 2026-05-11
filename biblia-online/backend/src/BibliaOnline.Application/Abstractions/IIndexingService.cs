namespace BibliaOnline.Application.Abstractions;

public interface IIndexingService
{
    Task EnsureSearchInfrastructureAsync(CancellationToken ct = default);
    Task ReindexAllVersesAsync(CancellationToken ct = default);
}
