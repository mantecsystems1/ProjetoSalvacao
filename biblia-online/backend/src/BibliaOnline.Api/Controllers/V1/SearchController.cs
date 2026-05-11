using BibliaOnline.Application.Abstractions;
using Microsoft.AspNetCore.Mvc;

namespace BibliaOnline.Api.Controllers.V1;

[ApiController]
[Route("api/v1/[controller]")]
public sealed class SearchController(IVerseSearchService search) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> Query([FromQuery] string q, [FromQuery] Guid? versionId, [FromQuery] int limit = 25, CancellationToken ct = default)
        => Ok(await search.SearchAsync(q, versionId, limit, ct));
}
