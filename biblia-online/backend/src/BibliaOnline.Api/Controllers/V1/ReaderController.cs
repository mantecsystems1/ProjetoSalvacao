using BibliaOnline.Application.Abstractions;
using Microsoft.AspNetCore.Mvc;

namespace BibliaOnline.Api.Controllers.V1;

[ApiController]
[Route("api/v1")]
public sealed class ReaderController(IBibleReaderService bible) : ControllerBase
{
    [HttpGet("versions/{versionId:guid}/books/{bookSlug}/chapters/{chapter:int}")]
    public async Task<IActionResult> Chapter(Guid versionId, string bookSlug, int chapter, CancellationToken ct)
    {
        var dto = await bible.GetChapterAsync(versionId, bookSlug, chapter, ct);
        return dto is null ? NotFound() : Ok(dto);
    }
}
