using Biblia.Application.Abstractions.Services;
using Biblia.Application.Services;
using Biblia.Application.Validation;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace Biblia.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddValidatorsFromAssemblyContaining<RegisterRequestValidator>();

        services.AddScoped<IBookAppService, BookAppService>();
        services.AddScoped<IChapterAppService, ChapterAppService>();
        services.AddScoped<IVerseQueryAppService, VerseQueryAppService>();
        services.AddScoped<IFavoriteAppService, FavoriteAppService>();
        services.AddScoped<IAuthAppService, AuthAppService>();

        return services;
    }
}
