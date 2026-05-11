using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace BibliaOnline.Infrastructure.Persistence;

public sealed class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
{
    public AppDbContext CreateDbContext(string[] args)
    {
        var env = Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT") ?? "Development";

        var dir = Directory.GetCurrentDirectory();
        while (dir is not null && !File.Exists(Path.Combine(dir, "appsettings.json")))
            dir = Directory.GetParent(dir)?.FullName;
        if (string.IsNullOrEmpty(dir))
            dir = Directory.GetCurrentDirectory();

        var config = new ConfigurationBuilder()
            .SetBasePath(dir)
            .AddJsonFile("appsettings.json", optional: false)
            .AddJsonFile($"appsettings.{env}.json", optional: true)
            .AddEnvironmentVariables()
            .Build();

        var cs = config.GetConnectionString("DefaultConnection")
                 ?? "Host=localhost;Port=5432;Database=biblia_online;Username=biblia;Password=biblia_dev";

        var opts = new DbContextOptionsBuilder<AppDbContext>()
            .UseNpgsql(cs);

        return new AppDbContext(opts.Options);
    }
}
