using BibliaOnline.Application.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BibliaOnline.Api.Controllers.V1;

/// <summary>Proteja em produção (ex.: API key header ou role admin).</summary>
[ApiController]
[Route("api/v1/admin")]
public sealed class AdminIndexingController(IIndexingService indexing) : ControllerBase
{
    [HttpPost("search/reindex")]
    [AllowAnonymous]
    public async Task<IActionResult> Reindex([FromHeader(Name = "X-Admin-Key")] string? adminKey, CancellationToken ct)
    {
        var expected = Environment.GetEnvironmentVariable("ADMIN_REINDEX_KEY");
        if (string.IsNullOrWhiteSpace(expected) || !string.Equals(adminKey, expected, StringComparison.Ordinal))
            return Unauthorized();

        await indexing.ReindexAllVersesAsync(ct);
        return Ok(new { ok = true });
    }
}
