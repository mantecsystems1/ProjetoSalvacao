using BibliaOnline.Domain.Common;

namespace BibliaOnline.Domain.Entities;

public class FavoriteVerse : Entity
{
    public Guid UserId { get; set; }
    public UserAccount User { get; set; } = null!;

    public Guid BibleVersionId { get; set; }
    public BibleVersion BibleVersion { get; set; } = null!;

    public Guid BookId { get; set; }
    public Book Book { get; set; } = null!;

    public int ChapterNumber { get; set; }
    public int VerseNumber { get; set; }
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
}
