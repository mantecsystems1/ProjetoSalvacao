using BibliaOnline.Domain.Common;

namespace BibliaOnline.Domain.Entities;

public class Verse : Entity
{
    public Guid BibleVersionId { get; set; }
    public BibleVersion BibleVersion { get; set; } = null!;

    public Guid BookId { get; set; }
    public Book Book { get; set; } = null!;

    public int ChapterNumber { get; set; }
    public int VerseNumber { get; set; }
    public string Text { get; set; } = string.Empty;
}
