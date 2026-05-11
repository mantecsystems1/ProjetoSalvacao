using Biblia.Domain.Common;

namespace Biblia.Domain.Entities;

public class Verse : Entity
{
    public Guid ChapterId { get; set; }
    public Chapter Chapter { get; set; } = null!;

    public Guid BibleVersionId { get; set; }
    public BibleVersion BibleVersion { get; set; } = null!;

    public int Number { get; set; }
    public string Text { get; set; } = string.Empty;
}
