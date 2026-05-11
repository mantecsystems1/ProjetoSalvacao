using Biblia.Application.Abstractions;
using Biblia.Application.Abstractions.Repositories;
using Biblia.Persistence.Repositories;
using Biblia.Persistence.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Biblia.Persistence;

public static class DependencyInjection
{
    public static IServiceCollection AddPersistence(this IServiceCollection services, IConfiguration configuration)
    {
        var cs = configuration.GetConnectionString("DefaultConnection")
                 ?? throw new InvalidOperationException("ConnectionStrings:DefaultConnection não configurada.");

        services.AddDbContext<BibliaDbContext>(o => o.UseNpgsql(cs));
        services.AddScoped<IUnitOfWork>(sp => sp.GetRequiredService<BibliaDbContext>());

        services.AddScoped<IBookRepository, BookRepository>();
        services.AddScoped<IChapterRepository, ChapterRepository>();
        services.AddScoped<IVerseRepository, VerseRepository>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IFavoriteVerseRepository, FavoriteVerseRepository>();

        services.AddScoped<SqlSearchAppService>();
        services.AddHostedService<DbInitializationHostedService>();

        return services;
    }
}
