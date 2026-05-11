using Biblia.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Biblia.Persistence.Configurations;

public sealed class ChapterConfiguration : IEntityTypeConfiguration<Chapter>
{
    public void Configure(EntityTypeBuilder<Chapter> b)
    {
        b.ToTable("chapters");
        b.HasKey(x => x.Id);
        b.HasIndex(x => new { x.BookId, x.Number }).IsUnique();
        b.HasOne(x => x.Book).WithMany(x => x.Chapters).HasForeignKey(x => x.BookId).OnDelete(DeleteBehavior.Cascade);
    }
}
