using Microsoft.EntityFrameworkCore;
using MyLists.Models;

namespace MyLists.Data;

public class MyListsDbContext(DbContextOptions<MyListsDbContext> options) : DbContext(options)
{
    public DbSet<TaskList> Lists => Set<TaskList>();
    public DbSet<TaskItem> Items => Set<TaskItem>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<TaskList>()
            .ToContainer("TaskLists")
            .HasPartitionKey(l => l.UserId)
            .HasKey(l => l.Id);

        modelBuilder.Entity<TaskItem>()
            .ToContainer("TaskItems")
            .HasPartitionKey(i => i.UserId)
            .HasKey(i => i.Id);
    }
}
