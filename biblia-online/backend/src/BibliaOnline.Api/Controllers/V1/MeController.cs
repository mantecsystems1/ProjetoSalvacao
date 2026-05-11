using BibliaOnline.Application.Abstractions;
using BibliaOnline.Application.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BibliaOnline.Api.Controllers.V1;

[ApiController]
[Authorize]
[Route("api/v1/me")]
public sealed class MeController(ICurrentUser current, IUserLibraryService library) : ControllerBase
{
    [HttpGet("favorites")]
    public async Task<IActionResult> Favorites(CancellationToken ct)
    {
        if (!TryUser(out var uid))
            return Unauthorized();
        return Ok(await library.GetFavoritesAsync(uid, ct));
    }

    [HttpPost("favorites")]
    public async Task<IActionResult> AddFavorite([FromBody] AddFavoriteRequest body, CancellationToken ct)
    {
        if (!TryUser(out var uid))
            return Unauthorized();
        try
        {
            var fav = await library.AddFavoriteAsync(uid, body, ct);
            return Ok(fav);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpDelete("favorites/{favoriteId:guid}")]
    public async Task<IActionResult> RemoveFavorite(Guid favoriteId, CancellationToken ct)
    {
        if (!TryUser(out var uid))
            return Unauthorized();
        try
        {
            await library.RemoveFavoriteAsync(uid, favoriteId, ct);
            return NoContent();
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }

    [HttpGet("reading-history")]
    public async Task<IActionResult> History([FromQuery] int take = 50, CancellationToken ct = default)
    {
        if (!TryUser(out var uid))
            return Unauthorized();
        return Ok(await library.GetHistoryAsync(uid, take, ct));
    }

    [HttpPost("reading-history")]
    public async Task<IActionResult> TouchHistory([FromBody] TouchHistoryRequest body, CancellationToken ct)
    {
        if (!TryUser(out var uid))
            return Unauthorized();
        try
        {
            await library.TouchHistoryAsync(uid, body, ct);
            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    private bool TryUser(out Guid uid)
    {
        uid = default;
        var id = current.UserId;
        if (!id.HasValue)
            return false;
        uid = id.Value;
        return true;
    }
}
