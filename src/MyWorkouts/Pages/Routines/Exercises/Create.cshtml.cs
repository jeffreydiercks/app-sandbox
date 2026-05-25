using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MyWorkouts.Data;
using MyWorkouts.Models;
using MyWorkouts.Security;

namespace MyWorkouts.Pages.Routines.Exercises;

public class ExerciseInput
{
    public string RoutineId { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public int DurationSeconds { get; set; } = 30;
    public int RestSeconds { get; set; } = 10;
    public int Order { get; set; } = 1;
    public string? Notes { get; set; }
}

public class CreateModel(MyWorkoutsDbContext db) : PageModel
{
    [BindProperty]
    public ExerciseInput Exercise { get; set; } = new();

    public IActionResult OnGet(string routineId)
    {
        Exercise.RoutineId = routineId;
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid) return Page();

        var userId = User.GetStableUserId();
        if (string.IsNullOrWhiteSpace(userId))
            return Forbid();

        var routine = await db.Routines.FindAsync(Exercise.RoutineId, userId);
        if (routine is null)
            return NotFound();

        routine.Exercises.Add(new Exercise
        {
            Id = Guid.NewGuid().ToString(),
            Name = Exercise.Name,
            DurationSeconds = Exercise.DurationSeconds,
            RestSeconds = Exercise.RestSeconds,
            Order = Exercise.Order,
            Notes = Exercise.Notes
        });
        routine.UpdatedAt = DateTime.UtcNow;

        await db.SaveChangesAsync();
        return RedirectToPage("/Routines/Details", new { id = Exercise.RoutineId });
    }
}
