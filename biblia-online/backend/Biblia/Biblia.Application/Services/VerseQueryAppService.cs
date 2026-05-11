using Biblia.Application.Abstractions.Repositories;
using Biblia.Application.Abstractions.Services;
using Biblia.Application.DTOs;

namespace Biblia.Application.Services;

public sealed class VerseQueryAppService(IVerseRepository verses, IChapterRepository chapters) : IVerseQueryAppService
{
    public async Task<IReadOnlyList<VerseListItemDto>> ListByChapterAsync(Guid chapterId, Guid bibleVersionId, CancellationToken cancellationToken = default)
    {
        if (await chapters.GetByIdAsync(chapterId, cancellationToken) is null)
            throw new KeyNotFoundException("Capítulo não encontrado.");

        var list = await verses.GetByChapterAndVersionAsync(chapterId, bibleVersionId, cancellationToken);
        return list.Select(v => new VerseListItemDto(v.Id, v.ChapterId, v.BibleVersionId, v.Number, v.Text)).ToList();
    }
}
