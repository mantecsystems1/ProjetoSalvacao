using Biblia.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Biblia.Persistence.Configurations;

public sealed class VerseConfiguration : IEntityTypeConfiguration<Verse>
{
    public void Configure(EntityTypeBuilder<Verse> b)
    {
        b.ToTable("verses");
        b.HasKey(x => x.Id);
        b.Property(x => x.Text).IsRequired();
        b.HasIndex(x => new { x.ChapterId, x.BibleVersionId, x.Number }).IsUnique();
        b.HasOne(x => x.Chapter).WithMany(x => x.Verses).HasForeignKey(x => x.ChapterId).OnDelete(DeleteBehavior.Cascade);
        b.HasOne(x => x.BibleVersion).WithMany(x => x.Verses).HasForeignKey(x => x.BibleVersionId).OnDelete(DeleteBehavior.Restrict);
    }
}
