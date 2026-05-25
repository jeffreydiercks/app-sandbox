using Microsoft.EntityFrameworkCore;
using MyWorkouts.Models;

namespace MyWorkouts.Data;

public class MyWorkoutsDbContext(DbContextOptions<MyWorkoutsDbContext> options) : DbContext(options)
{
    public DbSet<Routine> Routines => Set<Routine>();
    public DbSet<WorkoutSession> WorkoutSessions => Set<WorkoutSession>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Routine>()
            .ToContainer("Routines")
            .HasPartitionKey(r => r.UserId)
            .HasKey(r => new { r.Id, r.UserId });

        modelBuilder.Entity<Routine>()
            .OwnsMany(r => r.Exercises);

        modelBuilder.Entity<WorkoutSession>()
            .ToContainer("WorkoutSessions")
            .HasPartitionKey(s => s.UserId)
            .HasKey(s => new { s.Id, s.UserId });

        modelBuilder.Entity<WorkoutSession>()
            .OwnsMany(s => s.Entries);
    }
}
