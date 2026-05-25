using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MyWorkouts.Data;
using MyWorkouts.Models;
using MyWorkouts.Security;

namespace MyWorkouts.Pages.Routines;

public class EditModel(MyWorkoutsDbContext db) : PageModel
{
    [BindProperty]
    public Routine? Routine { get; set; }

    public async Task<IActionResult> OnGetAsync(string id)
    {
        var userId = User.GetStableUserId();
        if (string.IsNullOrWhiteSpace(userId))
            return Forbid();

        Routine = await db.Routines.FindAsync(id, userId);
        if (Routine is null)
            return NotFound();

        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid) return Page();

        var userId = User.GetStableUserId();
        if (string.IsNullOrWhiteSpace(userId))
            return Forbid();

        var existing = await db.Routines.FindAsync(Routine!.Id, userId);
        if (existing is null)
            return NotFound();

        existing.Name = Routine.Name;
        existing.Description = Routine.Description;
        existing.EstimatedMinutes = Routine.EstimatedMinutes;
        existing.UpdatedAt = DateTime.UtcNow;

        await db.SaveChangesAsync();
        return RedirectToPage("Details", new { id = existing.Id });
    }
}
