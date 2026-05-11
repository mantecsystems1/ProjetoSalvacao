using Biblia.API.Extensions;
using Biblia.API.Services;
using Biblia.Application;
using Biblia.Application.Abstractions;
using Biblia.Application.Abstractions.Search;
using Biblia.Application.Abstractions.Services;
using Biblia.Infrastructure;
using Biblia.Infrastructure.Search;
using Biblia.Persistence;
using Biblia.Persistence.Services;
using Microsoft.OpenApi.Models;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((ctx, cfg) => cfg.ReadFrom.Configuration(ctx.Configuration));

builder.Services.AddApplication();
builder.Services.AddPersistence(builder.Configuration);
builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.Configure<SearchOptions>(builder.Configuration.GetSection(SearchOptions.SectionName));
builder.Services.AddScoped<ISearchAppService>(sp =>
{
    var opt = sp.GetRequiredService<Microsoft.Extensions.Options.IOptions<SearchOptions>>().Value;
    return opt.Provider == SearchProviderKind.Meilisearch
        ? sp.GetRequiredService<MeilisearchSearchAppServiceStub>()
        : sp.GetRequiredService<SqlSearchAppService>();
});

builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ICurrentUser, HttpCurrentUser>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Biblia API", Version = "v1" });
    var scheme = new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header
    };
    c.AddSecurityDefinition("Bearer", scheme);
    c.AddSecurityRequirement(new OpenApiSecurityRequirement { [scheme] = Array.Empty<string>() });
});

var app = builder.Build();

app.UseGlobalExceptionHandler();

app.UseSerilogRequestLogging();

app.UseSwagger();
app.UseSwaggerUI();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
