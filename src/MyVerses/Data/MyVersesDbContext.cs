using Microsoft.EntityFrameworkCore;
using MyVerses.Models;

namespace MyVerses.Data;

public class MyVersesDbContext(DbContextOptions<MyVersesDbContext> options) : DbContext(options)
{
    public DbSet<Verse> Verses => Set<Verse>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Verse>()
            .ToContainer("Verses")
            .HasPartitionKey(v => v.UserId)
            .HasKey(v => v.Id);

        modelBuilder.Entity<Verse>()
            .Property(v => v.Category)
            .HasConversion<string>();
    }
}
