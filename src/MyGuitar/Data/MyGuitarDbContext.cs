using Microsoft.EntityFrameworkCore;
using MyGuitar.Models;

namespace MyGuitar.Data;

public class MyGuitarDbContext(DbContextOptions<MyGuitarDbContext> options) : DbContext(options)
{
    public DbSet<Lesson> Lessons => Set<Lesson>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Lesson>()
            .ToContainer("Lessons")
            .HasPartitionKey(l => l.UserId)
            .HasKey(l => l.Id);

        modelBuilder.Entity<Lesson>()
            .Property(l => l.Status)
            .HasConversion<string>();
    }
}
