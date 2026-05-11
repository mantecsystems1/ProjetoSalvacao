using BibliaOnline.Domain.Common;

namespace BibliaOnline.Domain.Entities;

public class Book : Entity
{
    /// <summary>Ordenação canônica protestante (1 = Gênesis … 66 = Apocalipse).</summary>
    public int CanonicalNumber { get; set; }

    public string Slug { get; set; } = string.Empty;

    public ICollection<BookTitle> Titles { get; set; } = new List<BookTitle>();
    public ICollection<Verse> Verses { get; set; } = new List<Verse>();
}
