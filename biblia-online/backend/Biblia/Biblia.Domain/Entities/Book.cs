using Biblia.Domain.Common;

namespace Biblia.Domain.Entities;

public class Book : Entity
{
    public int Order { get; set; }
    public string Slug { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;

    public ICollection<Chapter> Chapters { get; set; } = new List<Chapter>();
}
