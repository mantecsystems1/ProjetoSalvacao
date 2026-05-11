using Biblia.Application.Abstractions.Services;
using Biblia.Application.Common;
using Biblia.Application.DTOs;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;

namespace Biblia.API.Controllers;

[ApiController]
[Route("books")]
public sealed class BooksController(IBookAppService books, IChapterAppService chapters, IValidator<PageRequest> pageValidator) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(PagedResult<BookListItemDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> List([FromQuery] int page = 1, [FromQuery] int pageSize = 20, CancellationToken cancellationToken = default)
    {
        var pr = PageRequest.Create(page, pageSize);
        await pageValidator.ValidateAndThrowAsync(pr, cancellationToken);
        var result = await books.ListAsync(pr, cancellationToken);
        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(BookDetailDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var book = await books.GetByIdAsync(id, cancellationToken);
        return book is null ? NotFound() : Ok(book);
    }

    [HttpGet("{id:guid}/chapters")]
    [ProducesResponseType(typeof(IReadOnlyList<ChapterListItemDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> ListChapters(Guid id, CancellationToken cancellationToken)
    {
        var list = await chapters.ListByBookAsync(id, cancellationToken);
        return Ok(list);
    }
}
