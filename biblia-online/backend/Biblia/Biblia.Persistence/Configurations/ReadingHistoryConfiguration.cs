using Biblia.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Biblia.Persistence.Configurations;

public sealed class ReadingHistoryConfiguration : IEntityTypeConfiguration<ReadingHistory>
{
    public void Configure(EntityTypeBuilder<ReadingHistory> b)
    {
        b.ToTable("reading_history");
        b.HasKey(x => x.Id);
        b.HasIndex(x => new { x.UserId, x.BibleVersionId, x.ChapterId }).IsUnique();
        b.HasOne(x => x.User).WithMany(x => x.ReadingHistory).HasForeignKey(x => x.UserId).OnDelete(DeleteBehavior.Cascade);
        b.HasOne(x => x.BibleVersion).WithMany().HasForeignKey(x => x.BibleVersionId).OnDelete(DeleteBehavior.Restrict);
        b.HasOne(x => x.Chapter).WithMany().HasForeignKey(x => x.ChapterId).OnDelete(DeleteBehavior.Restrict);
    }
}
