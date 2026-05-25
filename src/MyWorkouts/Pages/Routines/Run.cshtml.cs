using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MyWorkouts.Data;
using MyWorkouts.Models;
using MyWorkouts.Security;

namespace MyWorkouts.Pages.Routines;

public class WorkoutLogInput
{
    public string ExerciseId { get; set; } = string.Empty;
    public string ExerciseName { get; set; } = string.Empty;
    public int SetsCompleted { get; set; }
    public int? ActualReps { get; set; }
    public decimal? ActualWeight { get; set; }
    public string? ActualWeightUnit { get; set; }
}

public class RunModel(MyWorkoutsDbContext db) : PageModel
{
    public Routine? Routine { get; private set; }

    [BindProperty]
    public List<WorkoutLogInput> Log { get; set; } = [];

    [BindProperty]
    public string? StartedAtUtc { get; set; }

    public async Task<IActionResult> OnGetAsync(string id)
    {
        var userId = User.GetStableUserId();
        if (string.IsNullOrWhiteSpace(userId))
            return Forbid();

        Routine = await db.Routines.FindAsync(id, userId);
        return Page();
    }

    public async Task<IActionResult> OnPostAsync(string id)
    {
        var userId = User.GetStableUserId();
        if (string.IsNullOrWhiteSpace(userId))
            return Forbid();

        var routine = await db.Routines.FindAsync(id, userId);
        if (routine is null)
            return NotFound();

        var startedAt = DateTime.TryParse(StartedAtUtc, out var parsed)
            ? parsed.ToUniversalTime()
            : DateTime.UtcNow;

        var exerciseLookup = routine.Exercises.ToDictionary(e => e.Id);

        var entries = Log
            .Where(l => exerciseLookup.ContainsKey(l.ExerciseId))
            .Select(l =>
            {
                var ex = exerciseLookup[l.ExerciseId];
                return new WorkoutLogEntry
                {
                    ExerciseId = ex.Id,
                    ExerciseName = ex.Name,
                    SetsCompleted = Math.Clamp(l.SetsCompleted, 0, ex.Sets),
                    ActualReps = l.ActualReps.HasValue ? Math.Clamp(l.ActualReps.Value, 0, 500) : null,
                    ActualWeight = l.ActualWeight.HasValue ? Math.Max(0, l.ActualWeight.Value) : null,
                    ActualWeightUnit = ex.WeightUnit
                };
            })
            .ToList();

        var session = new WorkoutSession
        {
            UserId = userId,
            RoutineId = id,
            RoutineName = routine.Name,
            StartedAt = startedAt,
            CompletedAt = DateTime.UtcNow,
            Entries = entries
        };

        db.WorkoutSessions.Add(session);
        await db.SaveChangesAsync();

        return RedirectToPage("/WorkoutSessions/Index");
    }
}

