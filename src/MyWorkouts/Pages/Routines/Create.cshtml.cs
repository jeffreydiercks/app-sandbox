using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MyWorkouts.Data;
using MyWorkouts.Models;
using MyWorkouts.Security;

namespace MyWorkouts.Pages.Routines;

public class CreateModel(MyWorkoutsDbContext db) : PageModel
{
    [BindProperty]
    public Routine Routine { get; set; } = new();

    public IActionResult OnGet() => Page();

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid) return Page();

        var userId = User.GetStableUserId();
        if (string.IsNullOrWhiteSpace(userId))
            return Forbid();

        Routine.Id = Guid.NewGuid().ToString();
        Routine.UserId = userId;
        Routine.CreatedAt = Routine.UpdatedAt = DateTime.UtcNow;

        db.Routines.Add(Routine);
        await db.SaveChangesAsync();

        return RedirectToPage("Details", new { id = Routine.Id });
    }
}
