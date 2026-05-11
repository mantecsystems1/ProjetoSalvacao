using BibliaOnline.Application.Abstractions;
using BibliaOnline.Application.Dtos;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;

namespace BibliaOnline.Api.Controllers.V1;

[ApiController]
[Route("api/v1/[controller]")]
public sealed class AuthController(IAuthService auth, IValidator<RegisterRequest> registerValidator, IValidator<LoginRequest> loginValidator) : ControllerBase
{
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest body, CancellationToken ct)
    {
        var vr = await registerValidator.ValidateAsync(body, ct);
        if (!vr.IsValid)
            return BadRequest(new { errors = vr.Errors.Select(e => new { e.PropertyName, e.ErrorMessage }) });

        try
        {
            var res = await auth.RegisterAsync(body, ct);
            return Ok(res);
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { message = ex.Message });
        }
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest body, CancellationToken ct)
    {
        var vr = await loginValidator.ValidateAsync(body, ct);
        if (!vr.IsValid)
            return BadRequest(new { errors = vr.Errors.Select(e => new { e.PropertyName, e.ErrorMessage }) });

        var res = await auth.LoginAsync(body, ct);
        return res is null ? Unauthorized(new { message = "Credenciais inválidas." }) : Ok(res);
    }
}
