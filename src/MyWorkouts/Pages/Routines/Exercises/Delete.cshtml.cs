using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MyWorkouts.Data;
using MyWorkouts.Security;

namespace MyWorkouts.Pages.Routines.Exercises;

public class DeleteModel(MyWorkoutsDbContext db) : PageModel
{
    public string? ExerciseName { get; private set; }
    public string ExerciseId { get; private set; } = string.Empty;
    public string RoutineId { get; private set; } = string.Empty;

    public async Task<IActionResult> OnGetAsync(string exerciseId, string routineId)
    {
        var userId = User.GetStableUserId();
        if (string.IsNullOrWhiteSpace(userId))
            return Forbid();

        var routine = await db.Routines.FindAsync(routineId, userId);
        if (routine is null)
            return NotFound();

        var ex = routine.Exercises.FirstOrDefault(e => e.Id == exerciseId);
        if (ex is null)
            return NotFound();

        ExerciseName = ex.Name;
        ExerciseId = exerciseId;
        RoutineId = routineId;
        return Page();
    }

    public async Task<IActionResult> OnPostAsync(string exerciseId, string routineId)
    {
        var userId = User.GetStableUserId();
        if (string.IsNullOrWhiteSpace(userId))
            return Forbid();

        var routine = await db.Routines.FindAsync(routineId, userId);
        if (routine is not null)
        {
            var ex = routine.Exercises.FirstOrDefault(e => e.Id == exerciseId);
            if (ex is not null)
            {
                routine.Exercises.Remove(ex);
                routine.UpdatedAt = DateTime.UtcNow;
                await db.SaveChangesAsync();
            }
        }

        return RedirectToPage("/Routines/Details", new { id = routineId });
    }
}
