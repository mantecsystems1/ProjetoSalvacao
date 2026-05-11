using Biblia.Domain.Common;

namespace Biblia.Domain.Entities;

public class BibleVersion : Entity
{
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;

    public ICollection<Verse> Verses { get; set; } = new List<Verse>();
}
