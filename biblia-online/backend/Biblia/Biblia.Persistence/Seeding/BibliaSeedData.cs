using Biblia.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Biblia.Persistence.Seeding;

public static class BibliaSeedData
{
    public static readonly Guid VersionSampleId = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaa1");
    public static readonly Guid BookGenesisId = Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbb1");
    public static readonly Guid ChapterGen1Id = Guid.Parse("cccccccc-cccc-cccc-cccc-ccccccccccc1");

    public static readonly Guid Verse1Id = Guid.Parse("dddddddd-dddd-dddd-dddd-dddddddddd01");
    public static readonly Guid Verse2Id = Guid.Parse("dddddddd-dddd-dddd-dddd-dddddddddd02");
    public static readonly Guid Verse3Id = Guid.Parse("dddddddd-dddd-dddd-dddd-dddddddddd03");

    public static async Task EnsureSeedAsync(BibliaDbContext db, CancellationToken ct)
    {
        if (await db.Books.AnyAsync(ct))
            return;

        db.BibleVersions.Add(new BibleVersion
        {
            Id = VersionSampleId,
            Code = "sample-pd",
            Name = "Versão de exemplo (domínio público)",
            IsActive = true
        });

        db.Books.Add(new Book
        {
            Id = BookGenesisId,
            Order = 1,
            Slug = "genesis",
            Name = "Gênesis"
        });

        db.Chapters.Add(new Chapter
        {
            Id = ChapterGen1Id,
            BookId = BookGenesisId,
            Number = 1
        });

        db.Verses.AddRange(
            new Verse
            {
                Id = Verse1Id,
                ChapterId = ChapterGen1Id,
                BibleVersionId = VersionSampleId,
                Number = 1,
                Text = "No princípio criou Deus os céus e a terra."
            },
            new Verse
            {
                Id = Verse2Id,
                ChapterId = ChapterGen1Id,
                BibleVersionId = VersionSampleId,
                Number = 2,
                Text = "A terra era sem forma e vazia; e havia trevas sobre a face do abismo."
            },
            new Verse
            {
                Id = Verse3Id,
                ChapterId = ChapterGen1Id,
                BibleVersionId = VersionSampleId,
                Number = 3,
                Text = "Disse Deus: Haja luz; e houve luz."
            });

        await db.SaveChangesAsync(ct);
    }
}
