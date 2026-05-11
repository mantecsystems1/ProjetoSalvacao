using BibliaOnline.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BibliaOnline.Infrastructure.Persistence.Configurations;

public class FavoriteVerseConfiguration : IEntityTypeConfiguration<FavoriteVerse>
{
    public void Configure(EntityTypeBuilder<FavoriteVerse> b)
    {
        b.ToTable("favorite_verses");
        b.HasKey(x => x.Id);
        b.HasIndex(x => new { x.UserId, x.BibleVersionId, x.BookId, x.ChapterNumber, x.VerseNumber }).IsUnique();
        b.HasOne(x => x.User).WithMany(x => x.Favorites).HasForeignKey(x => x.UserId).OnDelete(DeleteBehavior.Cascade);
        b.HasOne(x => x.BibleVersion).WithMany().HasForeignKey(x => x.BibleVersionId).OnDelete(DeleteBehavior.Restrict);
        b.HasOne(x => x.Book).WithMany().HasForeignKey(x => x.BookId).OnDelete(DeleteBehavior.Restrict);
    }
}
