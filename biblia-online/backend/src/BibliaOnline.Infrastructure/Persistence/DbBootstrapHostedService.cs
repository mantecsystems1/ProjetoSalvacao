using BibliaOnline.Application.Abstractions;
using BibliaOnline.Infrastructure.Search;
using BibliaOnline.Infrastructure.Persistence.Seed;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace BibliaOnline.Infrastructure.Persistence;

public sealed class DbBootstrapHostedService(
    IServiceProvider sp,
    ILogger<DbBootstrapHostedService> log) : IHostedService
{
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        await using var scope = sp.CreateAsyncScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var indexing = scope.ServiceProvider.GetRequiredService<IIndexingService>();
        var meili = scope.ServiceProvider.GetRequiredService<IOptions<MeilisearchOptions>>().Value;

        await db.Database.MigrateAsync(cancellationToken);

        if (!await db.Languages.AnyAsync(cancellationToken))
        {
            log.LogInformation("Banco vazio: aplicando seed mínimo (domínio público / demonstração).");
            await SampleBibleSeed.ApplyAsync(db, cancellationToken);
        }

        try
        {
            await indexing.EnsureSearchInfrastructureAsync(cancellationToken);
            if (meili.SyncOnStartup)
                await indexing.ReindexAllVersesAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            log.LogWarning(ex, "Falha ao sincronizar Meilisearch na inicialização; API continua sem busca.");
        }
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}
