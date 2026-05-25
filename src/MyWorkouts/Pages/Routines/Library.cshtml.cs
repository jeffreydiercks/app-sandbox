using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using MyWorkouts;
using MyWorkouts.Data;
using MyWorkouts.Models;
using MyWorkouts.Security;

namespace MyWorkouts.Pages.Routines;

public class LibraryModel(MyWorkoutsDbContext db) : PageModel
{
    public List<Routine> Templates { get; private set; } = [];

    public async Task OnGetAsync()
    {
        Templates = await db.Routines
            .Where(r => r.UserId == RoutineSeeder.SystemUserId)
            .OrderBy(r => r.Name)
            .ToListAsync();
    }

    public async Task<IActionResult> OnPostCopyAsync(string routineId)
    {
        var userId = User.GetStableUserId();
        if (string.IsNullOrWhiteSpace(userId))
            return Forbid();

        var template = await db.Routines
            .Where(r => r.UserId == RoutineSeeder.SystemUserId && r.Id == routineId)
            .FirstOrDefaultAsync();

        if (template is null)
            return NotFound();

        var copy = new Routine
        {
            UserId = userId,
            Name = template.Name,
            Description = template.Description,
            EstimatedMinutes = template.EstimatedMinutes,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            Exercises = template.Exercises.Select(e => new Exercise
            {
                Id = Guid.NewGuid().ToString(),
                Name = e.Name,
                IsRepBased = e.IsRepBased,
                DurationSeconds = e.DurationSeconds,
                Reps = e.Reps,
                Sets = e.Sets,
                IntraSetRestSeconds = e.IntraSetRestSeconds,
                RestSeconds = e.RestSeconds,
                PrescribedWeight = e.PrescribedWeight,
                WeightUnit = e.WeightUnit,
                Equipment = e.Equipment,
                IsOneSided = e.IsOneSided,
                Order = e.Order,
                Notes = e.Notes
            }).ToList()
        };

        db.Routines.Add(copy);
        await db.SaveChangesAsync();

        return RedirectToPage("/Routines/Index");
    }
}
