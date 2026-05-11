using Biblia.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Biblia.Persistence.Configurations;

public sealed class BibleVersionConfiguration : IEntityTypeConfiguration<BibleVersion>
{
    public void Configure(EntityTypeBuilder<BibleVersion> b)
    {
        b.ToTable("bible_versions");
        b.HasKey(x => x.Id);
        b.Property(x => x.Code).HasMaxLength(64).IsRequired();
        b.Property(x => x.Name).HasMaxLength(256).IsRequired();
        b.HasIndex(x => x.Code).IsUnique();
    }
}
