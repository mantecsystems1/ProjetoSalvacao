using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace Biblia.Persistence;

public sealed class BibliaDbContextFactory : IDesignTimeDbContextFactory<BibliaDbContext>
{
    public BibliaDbContext CreateDbContext(string[] args)
    {
        var env = Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT") ?? "Development";

        var dir = Directory.GetCurrentDirectory();
        while (dir is not null && !File.Exists(Path.Combine(dir, "appsettings.json")))
            dir = Directory.GetParent(dir)?.FullName;
        if (string.IsNullOrEmpty(dir))
            dir = Directory.GetCurrentDirectory();

        var configuration = new ConfigurationBuilder()
            .SetBasePath(dir)
            .AddJsonFile("appsettings.json", optional: false)
            .AddJsonFile($"appsettings.{env}.json", optional: true)
            .AddEnvironmentVariables()
            .Build();

        var cs = configuration.GetConnectionString("DefaultConnection")
                 ?? "Host=localhost;Port=5432;Database=biblia;Username=biblia;Password=biblia_dev";

        var options = new DbContextOptionsBuilder<BibliaDbContext>().UseNpgsql(cs).Options;
        return new BibliaDbContext(options);
    }
}
