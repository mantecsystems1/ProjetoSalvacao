using Biblia.Domain.Common;

namespace Biblia.Domain.Entities;

public class ReadingHistory : Entity
{
    public Guid UserId { get; set; }
    public User User { get; set; } = null!;

    public Guid BibleVersionId { get; set; }
    public BibleVersion BibleVersion { get; set; } = null!;

    public Guid ChapterId { get; set; }
    public Chapter Chapter { get; set; } = null!;

    public DateTime LastReadAtUtc { get; set; } = DateTime.UtcNow;
}
