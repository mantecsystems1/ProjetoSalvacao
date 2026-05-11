using BibliaOnline.Application.Abstractions;
using Microsoft.AspNetCore.Mvc;

namespace BibliaOnline.Api.Controllers.V1;

[ApiController]
[Route("api/v1/[controller]")]
public sealed class VersionsController(IBibleReaderService bible) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> List([FromQuery] Guid? languageId, CancellationToken ct)
        => Ok(await bible.GetVersionsAsync(languageId, ct));

    [HttpGet("by-code/{code}")]
    public async Task<IActionResult> ByCode(string code, CancellationToken ct)
    {
        var v = await bible.GetVersionByCodeAsync(code, ct);
        return v is null ? NotFound() : Ok(v);
    }

    [HttpGet("{versionId:guid}/books")]
    public async Task<IActionResult> Books(Guid versionId, CancellationToken ct)
        => Ok(await bible.GetBooksAsync(versionId, ct));
}
