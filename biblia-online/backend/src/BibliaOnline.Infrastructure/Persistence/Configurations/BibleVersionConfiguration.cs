using BibliaOnline.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BibliaOnline.Infrastructure.Persistence.Configurations;

public class BibleVersionConfiguration : IEntityTypeConfiguration<BibleVersion>
{
    public void Configure(EntityTypeBuilder<BibleVersion> b)
    {
        b.ToTable("bible_versions");
        b.HasKey(x => x.Id);
        b.Property(x => x.Code).HasMaxLength(64).IsRequired();
        b.Property(x => x.Name).HasMaxLength(256).IsRequired();
        b.Property(x => x.Description).HasMaxLength(1024);
        b.HasIndex(x => new { x.LanguageId, x.Code }).IsUnique();
        b.HasOne(x => x.Language).WithMany(x => x.Versions).HasForeignKey(x => x.LanguageId).OnDelete(DeleteBehavior.Restrict);
    }
}
