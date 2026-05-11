using BibliaOnline.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace BibliaOnline.Infrastructure.Persistence.Seed;

/// <summary>Dados mínimos para desenvolvimento. Substitua por importações completas em domínio público via scripts em imports/.</summary>
public static class SampleBibleSeed
{
    public static readonly Guid LanguagePt = Guid.Parse("11111111-1111-1111-1111-111111111101");
    public static readonly Guid LanguageEn = Guid.Parse("11111111-1111-1111-1111-111111111102");
    public static readonly Guid VersionPtSample = Guid.Parse("22222222-2222-2222-2222-222222222201");
    public static readonly Guid VersionEnSample = Guid.Parse("22222222-2222-2222-2222-222222222202");
    public static readonly Guid BookGenesis = Guid.Parse("33333333-3333-3333-3333-333333333301");

    public static async Task ApplyAsync(AppDbContext db, CancellationToken ct)
    {
        db.Languages.AddRange(
            new Language
            {
                Id = LanguagePt,
                Code = "pt",
                NativeName = "Português",
                EnglishName = "Portuguese",
                IsRtl = false
            },
            new Language
            {
                Id = LanguageEn,
                Code = "en",
                NativeName = "English",
                EnglishName = "English",
                IsRtl = false
            });

        db.BibleVersions.AddRange(
            new BibleVersion
            {
                Id = VersionPtSample,
                LanguageId = LanguagePt,
                Code = "pt-pd-sample",
                Name = "Amostra PT (domínio público)",
                Description = "Texto curto para desenvolvimento — importe versão completa licenciada adequadamente.",
                IsActive = true
            },
            new BibleVersion
            {
                Id = VersionEnSample,
                LanguageId = LanguageEn,
                Code = "web-sample",
                Name = "World English Bible (amostra)",
                Description = "Trechos de exemplo; WEB é domínio público.",
                IsActive = true
            });

        db.Books.Add(new Book
        {
            Id = BookGenesis,
            CanonicalNumber = 1,
            Slug = "genesis"
        });

        db.BookTitles.AddRange(
            new BookTitle { BibleVersionId = VersionPtSample, BookId = BookGenesis, Title = "Gênesis" },
            new BookTitle { BibleVersionId = VersionEnSample, BookId = BookGenesis, Title = "Genesis" });

        var ptVerses = new[]
        {
            (1, "No princípio criou Deus os céus e a terra."),
            (2, "A terra era sem forma e vazia; e havia trevas sobre a face do abismo."),
            (3, "Disse Deus: Haja luz; e houve luz.")
        };

        var enVerses = new[]
        {
            (1, "In the beginning, God created the heavens and the earth."),
            (2, "The earth was formless and empty. Darkness was on the surface of the deep."),
            (3, "God said, \"Let there be light,\" and there was light.")
        };

        foreach (var (n, text) in ptVerses)
        {
            db.Verses.Add(new Verse
            {
                BibleVersionId = VersionPtSample,
                BookId = BookGenesis,
                ChapterNumber = 1,
                VerseNumber = n,
                Text = text
            });
        }

        foreach (var (n, text) in enVerses)
        {
            db.Verses.Add(new Verse
            {
                BibleVersionId = VersionEnSample,
                BookId = BookGenesis,
                ChapterNumber = 1,
                VerseNumber = n,
                Text = text
            });
        }

        await db.SaveChangesAsync(ct);
    }
}
