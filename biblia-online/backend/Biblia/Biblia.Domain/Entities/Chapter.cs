using Biblia.Domain.Common;

namespace Biblia.Domain.Entities;

public class Chapter : Entity
{
    public Guid BookId { get; set; }
    public Book Book { get; set; } = null!;
    public int Number { get; set; }

    public ICollection<Verse> Verses { get; set; } = new List<Verse>();
}
