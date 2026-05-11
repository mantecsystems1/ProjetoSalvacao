using BibliaOnline.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BibliaOnline.Infrastructure.Persistence.Configurations;

public class BookTitleConfiguration : IEntityTypeConfiguration<BookTitle>
{
    public void Configure(EntityTypeBuilder<BookTitle> b)
    {
        b.ToTable("book_titles");
        b.HasKey(x => x.Id);
        b.Property(x => x.Title).HasMaxLength(128).IsRequired();
        b.HasIndex(x => new { x.BibleVersionId, x.BookId }).IsUnique();
        b.HasOne(x => x.BibleVersion).WithMany(x => x.BookTitles).HasForeignKey(x => x.BibleVersionId).OnDelete(DeleteBehavior.Cascade);
        b.HasOne(x => x.Book).WithMany(x => x.Titles).HasForeignKey(x => x.BookId).OnDelete(DeleteBehavior.Cascade);
    }
}
