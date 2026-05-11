using BibliaOnline.Application.Abstractions;
using BibliaOnline.Infrastructure.Persistence;
using BibliaOnline.Infrastructure.Search;
using BibliaOnline.Infrastructure.Security;
using BibliaOnline.Infrastructure.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace BibliaOnline.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<JwtOptions>(configuration.GetSection(JwtOptions.SectionName));
        services.Configure<MeilisearchOptions>(configuration.GetSection(MeilisearchOptions.SectionName));

        var conn = configuration.GetConnectionString("DefaultConnection")
                   ?? throw new InvalidOperationException("ConnectionStrings:DefaultConnection não configurada.");

        services.AddDbContext<AppDbContext>(o => o.UseNpgsql(conn));

        services.AddSingleton<IJwtTokenIssuer, JwtTokenIssuer>();
        services.AddScoped<IBibleReaderService, BibleReaderService>();
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IUserLibraryService, UserLibraryService>();
        services.AddScoped<IVerseSearchService, VerseSearchService>();
        services.AddScoped<IIndexingService, IndexingService>();

        var jwt = configuration.GetSection(JwtOptions.SectionName).Get<JwtOptions>() ?? new JwtOptions();
        if (string.IsNullOrWhiteSpace(jwt.SigningKey) || jwt.SigningKey.Length < 32)
            throw new InvalidOperationException("Jwt:SigningKey deve ter pelo menos 32 caracteres.");

        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(o =>
            {
                o.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwt.Issuer,
                    ValidAudience = jwt.Audience,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt.SigningKey))
                };
            });

        services.AddAuthorization();

        services.AddHostedService<DbBootstrapHostedService>();

        return services;
    }
}
