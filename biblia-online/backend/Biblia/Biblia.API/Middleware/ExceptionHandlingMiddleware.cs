using System.Net;
using System.Text.Json;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;

namespace Biblia.API.Middleware;

public sealed class ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger, IHostEnvironment env)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Erro não tratado: {Message}", ex.Message);
            await WriteProblemAsync(context, ex);
        }
    }

    private Task WriteProblemAsync(HttpContext context, Exception exception)
    {
        var (status, title, detail, errors) = Map(exception);

        var problem = new ProblemDetails
        {
            Status = status,
            Title = title,
            Detail = env.IsDevelopment() ? detail : title,
            Instance = context.Request.Path
        };

        if (errors is not null)
            problem.Extensions["errors"] = errors;

        context.Response.ContentType = "application/problem+json";
        context.Response.StatusCode = status;

        var json = JsonSerializer.Serialize(problem, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
        return context.Response.WriteAsync(json);
    }

    private static (int Status, string Title, string Detail, IDictionary<string, string[]>? Errors) Map(Exception exception) =>
        exception switch
        {
            ValidationException vex => (
                (int)HttpStatusCode.BadRequest,
                "Erro de validação.",
                vex.Message,
                vex.Errors
                    .GroupBy(e => e.PropertyName)
                    .ToDictionary(g => g.Key, g => g.Select(x => x.ErrorMessage).ToArray())
            ),
            KeyNotFoundException knf => (
                (int)HttpStatusCode.NotFound,
                knf.Message,
                knf.Message,
                null
            ),
            UnauthorizedAccessException => (
                (int)HttpStatusCode.Unauthorized,
                "Não autorizado.",
                exception.Message,
                null
            ),
            InvalidOperationException ioe => (
                (int)HttpStatusCode.Conflict,
                ioe.Message,
                ioe.Message,
                null
            ),
            _ => (
                (int)HttpStatusCode.InternalServerError,
                "Erro interno no servidor.",
                exception.Message,
                null
            )
        };
}
