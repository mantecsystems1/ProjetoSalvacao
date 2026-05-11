using Biblia.Application.Abstractions.Services;
using Biblia.Application.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace Biblia.API.Controllers;

[ApiController]
[Route("chapters")]
public sealed class ChaptersController(IVerseQueryAppService verses) : ControllerBase
{
    [HttpGet("{id:guid}/verses")]
    [ProducesResponseType(typeof(IReadOnlyList<VerseListItemDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ListVerses(Guid id, [FromQuery] Guid versionId, CancellationToken cancellationToken)
    {
        if (versionId == Guid.Empty)
            return BadRequest(new { message = "Query obrigatória: versionId." });

        var list = await verses.ListByChapterAsync(id, versionId, cancellationToken);
        return Ok(list);
    }
}
