using BibliaOnline.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BibliaOnline.Infrastructure.Persistence.Configurations;

public class ReadingHistoryEntryConfiguration : IEntityTypeConfiguration<ReadingHistoryEntry>
{
    public void Configure(EntityTypeBuilder<ReadingHistoryEntry> b)
    {
        b.ToTable("reading_history");
        b.HasKey(x => x.Id);
        b.HasIndex(x => new { x.UserId, x.BibleVersionId, x.BookId, x.ChapterNumber }).IsUnique();
        b.HasOne(x => x.User).WithMany(x => x.ReadingHistory).HasForeignKey(x => x.UserId).OnDelete(DeleteBehavior.Cascade);
        b.HasOne(x => x.BibleVersion).WithMany().HasForeignKey(x => x.BibleVersionId).OnDelete(DeleteBehavior.Restrict);
        b.HasOne(x => x.Book).WithMany().HasForeignKey(x => x.BookId).OnDelete(DeleteBehavior.Restrict);
    }
}
