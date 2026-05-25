using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MyWorkouts.Data;
using MyWorkouts.Models;
using MyWorkouts.Security;

namespace MyWorkouts.Pages.Routines.Exercises;

public class ExerciseEditInput
{
    public string Name { get; set; } = string.Empty;
    public int DurationSeconds { get; set; } = 30;
    public int RestSeconds { get; set; } = 10;
    public int Order { get; set; } = 1;
    public string? Notes { get; set; }
}

public class EditModel(MyWorkoutsDbContext db) : PageModel
{
    [BindProperty]
    public ExerciseEditInput? Exercise { get; set; }
    public string RoutineId { get; private set; } = string.Empty;
    private string _exerciseId = string.Empty;

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

        RoutineId = routineId;
        Exercise = new ExerciseEditInput
        {
            Name = ex.Name,
            DurationSeconds = ex.DurationSeconds,
            RestSeconds = ex.RestSeconds,
            Order = ex.Order,
            Notes = ex.Notes
        };

        TempData["exerciseId"] = exerciseId;
        TempData["routineId"] = routineId;

        return Page();
    }

    public async Task<IActionResult> OnPostAsync(string exerciseId, string routineId)
    {
        if (!ModelState.IsValid)
        {
            RoutineId = routineId;
            return Page();
        }

        var userId = User.GetStableUserId();
        if (string.IsNullOrWhiteSpace(userId))
            return Forbid();

        var routine = await db.Routines.FindAsync(routineId, userId);
        if (routine is null)
            return NotFound();

        var ex = routine.Exercises.FirstOrDefault(e => e.Id == exerciseId);
        if (ex is null)
            return NotFound();

        ex.Name = Exercise!.Name;
        ex.DurationSeconds = Exercise.DurationSeconds;
        ex.RestSeconds = Exercise.RestSeconds;
        ex.Order = Exercise.Order;
        ex.Notes = Exercise.Notes;
        routine.UpdatedAt = DateTime.UtcNow;

        await db.SaveChangesAsync();
        return RedirectToPage("/Routines/Details", new { id = routineId });
    }
}
