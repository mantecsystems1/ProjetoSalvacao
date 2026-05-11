using BibliaOnline.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BibliaOnline.Infrastructure.Persistence.Configurations;

public class VerseConfiguration : IEntityTypeConfiguration<Verse>
{
    public void Configure(EntityTypeBuilder<Verse> b)
    {
        b.ToTable("verses");
        b.HasKey(x => x.Id);
        b.Property(x => x.Text).IsRequired();
        b.HasIndex(x => new { x.BibleVersionId, x.BookId, x.ChapterNumber, x.VerseNumber }).IsUnique();
        b.HasOne(x => x.BibleVersion).WithMany(x => x.Verses).HasForeignKey(x => x.BibleVersionId).OnDelete(DeleteBehavior.Cascade);
        b.HasOne(x => x.Book).WithMany(x => x.Verses).HasForeignKey(x => x.BookId).OnDelete(DeleteBehavior.Cascade);
    }
}
