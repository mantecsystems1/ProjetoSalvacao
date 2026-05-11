using Biblia.Application.Abstractions;
using Biblia.Application.Abstractions.Services;
using Biblia.Application.Common;
using Biblia.Application.DTOs;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Biblia.API.Controllers;

[ApiController]
[Authorize]
[Route("favorites")]
public sealed class FavoritesController(
    IFavoriteAppService favorites,
    ICurrentUser currentUser,
    IValidator<PageRequest> pageValidator,
    IValidator<CreateFavoriteRequest> favoriteValidator) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(PagedResult<FavoriteListItemDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> List([FromQuery] int page = 1, [FromQuery] int pageSize = 20, CancellationToken cancellationToken = default)
    {
        var uid = RequireUserId();
        var pr = PageRequest.Create(page, pageSize);
        await pageValidator.ValidateAndThrowAsync(pr, cancellationToken);
        var result = await favorites.ListAsync(uid, pr, cancellationToken);
        return Ok(result);
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Create([FromBody] CreateFavoriteRequest body, CancellationToken cancellationToken)
    {
        await favoriteValidator.ValidateAndThrowAsync(body, cancellationToken);
        var uid = RequireUserId();
        await favorites.AddAsync(uid, body, cancellationToken);
        return NoContent();
    }

    private Guid RequireUserId()
    {
        var id = currentUser.UserId;
        if (!id.HasValue)
            throw new UnauthorizedAccessException();
        return id.Value;
    }
}
