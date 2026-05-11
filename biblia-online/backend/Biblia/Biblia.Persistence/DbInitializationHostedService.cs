using Biblia.Persistence.Seeding;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Biblia.Persistence;

public sealed class DbInitializationHostedService(IServiceProvider serviceProvider, ILogger<DbInitializationHostedService> logger) : IHostedService
{
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        await using var scope = serviceProvider.CreateAsyncScope();
        var db = scope.ServiceProvider.GetRequiredService<BibliaDbContext>();

        await db.Database.MigrateAsync(cancellationToken);
        await BibliaSeedData.EnsureSeedAsync(db, cancellationToken);
        logger.LogInformation("Banco migrado e seed verificado.");
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}
