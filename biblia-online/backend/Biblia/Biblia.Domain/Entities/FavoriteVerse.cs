using Biblia.Domain.Common;

namespace Biblia.Domain.Entities;

public class FavoriteVerse : Entity
{
    public Guid UserId { get; set; }
    public User User { get; set; } = null!;

    public Guid VerseId { get; set; }
    public Verse Verse { get; set; } = null!;

    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
}
