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

        var session = new WorkoutSession
        {
            UserId = userId,
            RoutineId = id,
            RoutineName = routine.Name,
            StartedAt = startedAt,
            CompletedAt = DateTime.UtcNow,
            Entries = Log.Select(l => new WorkoutLogEntry
            {
                ExerciseId = l.ExerciseId,
                ExerciseName = l.ExerciseName,
                SetsCompleted = l.SetsCompleted,
                ActualReps = l.ActualReps,
                ActualWeight = l.ActualWeight,
                ActualWeightUnit = l.ActualWeightUnit
            }).ToList()
        };

        db.WorkoutSessions.Add(session);
        await db.SaveChangesAsync();

        return RedirectToPage("/WorkoutSessions/Index");
    }
}

