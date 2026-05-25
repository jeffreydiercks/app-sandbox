using Microsoft.EntityFrameworkCore;
using MyReader.Models;

namespace MyReader.Data;

public class MyReaderDbContext(DbContextOptions<MyReaderDbContext> options) : DbContext(options)
{
    public DbSet<Feed> Feeds => Set<Feed>();
    public DbSet<ReadItem> ReadItems => Set<ReadItem>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Feed>()
            .ToContainer("Feeds")
            .HasPartitionKey(f => f.UserId)
            .HasKey(f => f.Id);

        modelBuilder.Entity<ReadItem>()
            .ToContainer("ReadItems")
            .HasPartitionKey(r => r.UserId)
            .HasKey(r => r.Id);
    }
}
