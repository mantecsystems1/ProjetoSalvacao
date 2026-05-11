using Biblia.Application.Abstractions.Services;
using Biblia.Application.DTOs;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;

namespace Biblia.API.Controllers;

[ApiController]
[Route("auth")]
public sealed class AuthController(
    IAuthAppService auth,
    IValidator<RegisterRequest> registerValidator,
    IValidator<LoginRequest> loginValidator) : ControllerBase
{
    [HttpPost("register")]
    [ProducesResponseType(typeof(AuthResponseDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> Register([FromBody] RegisterRequest body, CancellationToken cancellationToken)
    {
        await registerValidator.ValidateAndThrowAsync(body, cancellationToken);
        try
        {
            var result = await auth.RegisterAsync(body, cancellationToken);
            return Ok(result);
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { message = ex.Message });
        }
    }

    [HttpPost("login")]
    [ProducesResponseType(typeof(AuthResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Login([FromBody] LoginRequest body, CancellationToken cancellationToken)
    {
        await loginValidator.ValidateAndThrowAsync(body, cancellationToken);
        var result = await auth.LoginAsync(body, cancellationToken);
        return result is null ? Unauthorized(new { message = "Credenciais inválidas." }) : Ok(result);
    }
}
