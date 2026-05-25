using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using MyWorkouts.Data;
using MyWorkouts.Models;
using MyWorkouts.Security;

namespace MyWorkouts.Pages;

public class IndexModel(MyWorkoutsDbContext db) : PageModel
{
    public List<Routine> Routines { get; private set; } = [];

    public async Task<IActionResult> OnGetAsync()
    {
        var userId = User.GetStableUserId();
        if (string.IsNullOrWhiteSpace(userId))
            return Forbid();

        Routines = await db.Routines
            .Where(r => r.UserId == userId)
            .OrderBy(r => r.Name)
            .ToListAsync();

        return Page();
    }
}
