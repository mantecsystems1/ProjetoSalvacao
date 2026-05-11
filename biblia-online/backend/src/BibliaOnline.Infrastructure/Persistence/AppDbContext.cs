using BibliaOnline.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace BibliaOnline.Infrastructure.Persistence;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Language> Languages => Set<Language>();
    public DbSet<BibleVersion> BibleVersions => Set<BibleVersion>();
    public DbSet<Book> Books => Set<Book>();
    public DbSet<BookTitle> BookTitles => Set<BookTitle>();
    public DbSet<Verse> Verses => Set<Verse>();
    public DbSet<UserAccount> Users => Set<UserAccount>();
    public DbSet<FavoriteVerse> FavoriteVerses => Set<FavoriteVerse>();
    public DbSet<ReadingHistoryEntry> ReadingHistory => Set<ReadingHistoryEntry>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}
