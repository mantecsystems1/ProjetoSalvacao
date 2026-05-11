using Biblia.Application.Abstractions.Services;
using Biblia.Application.Common;
using Biblia.Application.DTOs;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;

namespace Biblia.API.Controllers;

[ApiController]
[Route("search")]
public sealed class SearchController(ISearchAppService search, IValidator<PageRequest> pageValidator) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(PagedResult<VerseSearchResultDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Query([FromQuery] string q, [FromQuery] Guid? versionId, [FromQuery] int page = 1, [FromQuery] int pageSize = 20, CancellationToken cancellationToken = default)
    {
        var pr = PageRequest.Create(page, pageSize);
        await pageValidator.ValidateAndThrowAsync(pr, cancellationToken);
        var result = await search.SearchAsync(q, versionId, pr, cancellationToken);
        return Ok(result);
    }
}
