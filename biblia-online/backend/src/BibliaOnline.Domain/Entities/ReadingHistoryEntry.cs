using BibliaOnline.Domain.Common;

namespace BibliaOnline.Domain.Entities;

public class ReadingHistoryEntry : Entity
{
    public Guid UserId { get; set; }
    public UserAccount User { get; set; } = null!;

    public Guid BibleVersionId { get; set; }
    public BibleVersion BibleVersion { get; set; } = null!;

    public Guid BookId { get; set; }
    public Book Book { get; set; } = null!;

    public int ChapterNumber { get; set; }
    public DateTime LastReadAtUtc { get; set; } = DateTime.UtcNow;
}
