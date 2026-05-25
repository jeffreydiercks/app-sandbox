using Microsoft.EntityFrameworkCore;
using MyWorkouts.Data;
using MyWorkouts.Models;

namespace MyWorkouts;

public static class RoutineSeeder
{
    public const string SystemUserId = "__system__";

    public static async Task SeedSystemRoutinesAsync(IServiceProvider services)
    {
        using var scope = services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<MyWorkoutsDbContext>();

        var alreadySeeded = (await db.Routines
            .Where(r => r.UserId == SystemUserId)
            .Take(1)
            .ToListAsync()).Count > 0;

        if (alreadySeeded) return;

        var sevenMinute = new Routine
        {
            UserId = SystemUserId,
            Name = "7-Minute Workout",
            Description = "The classic high-intensity circuit — 12 exercises, 30 seconds each with 10 seconds rest.",
            EstimatedMinutes = 7,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            Exercises =
            [
                new() { Order = 1,  Name = "Jumping Jacks",           DurationSeconds = 30, RestSeconds = 10, Equipment = EquipmentType.Bodyweight },
                new() { Order = 2,  Name = "Wall Sit",                 DurationSeconds = 30, RestSeconds = 10, Equipment = EquipmentType.Wall },
                new() { Order = 3,  Name = "Push-Ups",                 DurationSeconds = 30, RestSeconds = 10, Equipment = EquipmentType.Bodyweight },
                new() { Order = 4,  Name = "Abdominal Crunches",       DurationSeconds = 30, RestSeconds = 10, Equipment = EquipmentType.Bodyweight },
                new() { Order = 5,  Name = "Step-Up onto Chair",       DurationSeconds = 30, RestSeconds = 10, Equipment = EquipmentType.ChairBench },
                new() { Order = 6,  Name = "Squats",                   DurationSeconds = 30, RestSeconds = 10, Equipment = EquipmentType.Bodyweight },
                new() { Order = 7,  Name = "Triceps Dip on Chair",     DurationSeconds = 30, RestSeconds = 10, Equipment = EquipmentType.ChairBench },
                new() { Order = 8,  Name = "Plank",                    DurationSeconds = 30, RestSeconds = 10, Equipment = EquipmentType.Bodyweight },
                new() { Order = 9,  Name = "High Knees",               DurationSeconds = 30, RestSeconds = 10, Equipment = EquipmentType.Bodyweight },
                new() { Order = 10, Name = "Lunges",                   DurationSeconds = 30, RestSeconds = 10, Equipment = EquipmentType.Bodyweight },
                new() { Order = 11, Name = "Push-Up with Rotation",    DurationSeconds = 30, RestSeconds = 10, Equipment = EquipmentType.Bodyweight },
                new() { Order = 12, Name = "Side Plank",               DurationSeconds = 30, RestSeconds = 0,  Equipment = EquipmentType.Bodyweight, IsOneSided = true },
            ]
        };

        db.Routines.Add(sevenMinute);
        await db.SaveChangesAsync();
    }
}
