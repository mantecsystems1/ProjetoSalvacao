using BibliaOnline.Application.Abstractions;
using Microsoft.AspNetCore.Mvc;

namespace BibliaOnline.Api.Controllers.V1;

[ApiController]
[Route("api/v1/[controller]")]
public sealed class LanguagesController(IBibleReaderService bible) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> List(CancellationToken ct)
        => Ok(await bible.GetLanguagesAsync(ct));
}
