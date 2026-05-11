using BibliaOnline.Domain.Common;

namespace BibliaOnline.Domain.Entities;

public class UserAccount : Entity
{
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;

    public ICollection<FavoriteVerse> Favorites { get; set; } = new List<FavoriteVerse>();
    public ICollection<ReadingHistoryEntry> ReadingHistory { get; set; } = new List<ReadingHistoryEntry>();
}
