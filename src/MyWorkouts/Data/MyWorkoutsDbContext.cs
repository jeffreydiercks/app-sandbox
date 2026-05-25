using Microsoft.EntityFrameworkCore;
using MyWorkouts.Models;

namespace MyWorkouts.Data;

public class MyWorkoutsDbContext(DbContextOptions<MyWorkoutsDbContext> options) : DbContext(options)
{
    public DbSet<Routine> Routines => Set<Routine>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Routine>()
            .ToContainer("Routines")
            .HasPartitionKey(r => r.UserId)
            .HasKey(r => r.Id);

        modelBuilder.Entity<Routine>()
            .OwnsMany(r => r.Exercises);
    }
}
