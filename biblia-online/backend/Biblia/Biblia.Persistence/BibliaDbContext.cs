using Biblia.Application.Abstractions;
using Biblia.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Biblia.Persistence;

public sealed class BibliaDbContext(DbContextOptions<BibliaDbContext> options) : DbContext(options), IUnitOfWork
{
    public DbSet<BibleVersion> BibleVersions => Set<BibleVersion>();
    public DbSet<Book> Books => Set<Book>();
    public DbSet<Chapter> Chapters => Set<Chapter>();
    public DbSet<Verse> Verses => Set<Verse>();
    public DbSet<User> Users => Set<User>();
    public DbSet<FavoriteVerse> FavoriteVerses => Set<FavoriteVerse>();
    public DbSet<ReadingHistory> ReadingHistories => Set<ReadingHistory>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(BibliaDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}
