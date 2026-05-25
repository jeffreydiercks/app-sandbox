using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MyWorkouts.Data;
using MyWorkouts.Models;
using MyWorkouts.Security;

namespace MyWorkouts.Pages.Routines.Exercises;

public class ExerciseEditInput
{
    public string Name { get; set; } = string.Empty;
    public bool IsRepBased { get; set; } = false;
    public int DurationSeconds { get; set; } = 30;
    public int Reps { get; set; } = 10;
    public int Sets { get; set; } = 1;
    public int IntraSetRestSeconds { get; set; } = 30;
    public int RestSeconds { get; set; } = 10;
    public decimal? PrescribedWeight { get; set; }
    public string WeightUnit { get; set; } = "lbs";
    public EquipmentType Equipment { get; set; } = EquipmentType.Bodyweight;
    public bool IsOneSided { get; set; } = false;
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
            IsRepBased = ex.IsRepBased,
            DurationSeconds = ex.DurationSeconds,
            Reps = ex.Reps,
            Sets = ex.Sets,
            IntraSetRestSeconds = ex.IntraSetRestSeconds,
            RestSeconds = ex.RestSeconds,
            PrescribedWeight = ex.PrescribedWeight,
            WeightUnit = ex.WeightUnit,
            Equipment = ex.Equipment,
            IsOneSided = ex.IsOneSided,
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
        ex.IsRepBased = Exercise.IsRepBased;
        ex.DurationSeconds = Exercise.DurationSeconds;
        ex.Reps = Exercise.Reps;
        ex.Sets = Exercise.Sets;
        ex.IntraSetRestSeconds = Exercise.IntraSetRestSeconds;
        ex.RestSeconds = Exercise.RestSeconds;
        ex.PrescribedWeight = Exercise.PrescribedWeight;
        ex.WeightUnit = Exercise.WeightUnit;
        ex.Equipment = Exercise.Equipment;
        ex.IsOneSided = Exercise.IsOneSided;
        ex.Order = Exercise.Order;
        ex.Notes = Exercise.Notes;
        routine.UpdatedAt = DateTime.UtcNow;

        await db.SaveChangesAsync();
        return RedirectToPage("/Routines/Details", new { id = routineId });
    }
}
