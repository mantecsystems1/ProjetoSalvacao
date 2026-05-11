using Biblia.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Biblia.Persistence.Configurations;

public sealed class FavoriteVerseConfiguration : IEntityTypeConfiguration<FavoriteVerse>
{
    public void Configure(EntityTypeBuilder<FavoriteVerse> b)
    {
        b.ToTable("favorite_verses");
        b.HasKey(x => x.Id);
        b.HasIndex(x => new { x.UserId, x.VerseId }).IsUnique();
        b.HasOne(x => x.User).WithMany(x => x.Favorites).HasForeignKey(x => x.UserId).OnDelete(DeleteBehavior.Cascade);
        b.HasOne(x => x.Verse).WithMany().HasForeignKey(x => x.VerseId).OnDelete(DeleteBehavior.Restrict);
    }
}
