using BibliaOnline.Domain.Common;

namespace BibliaOnline.Domain.Entities;

public class Language : Entity
{
    public string Code { get; set; } = string.Empty;
    public string NativeName { get; set; } = string.Empty;
    public string EnglishName { get; set; } = string.Empty;
    public bool IsRtl { get; set; }

    public ICollection<BibleVersion> Versions { get; set; } = new List<BibleVersion>();
}
