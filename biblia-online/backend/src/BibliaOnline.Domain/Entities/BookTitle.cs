using BibliaOnline.Domain.Common;

namespace BibliaOnline.Domain.Entities;

public class BookTitle : Entity
{
    public Guid BibleVersionId { get; set; }
    public BibleVersion BibleVersion { get; set; } = null!;

    public Guid BookId { get; set; }
    public Book Book { get; set; } = null!;

    public string Title { get; set; } = string.Empty;
}
