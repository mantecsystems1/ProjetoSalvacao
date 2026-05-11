using BibliaOnline.Domain.Common;

namespace BibliaOnline.Domain.Entities;

public class BibleVersion : Entity
{
    public Guid LanguageId { get; set; }
    public Language Language { get; set; } = null!;

    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool IsActive { get; set; } = true;

    public ICollection<BookTitle> BookTitles { get; set; } = new List<BookTitle>();
    public ICollection<Verse> Verses { get; set; } = new List<Verse>();
}
