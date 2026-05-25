using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using MyWorkouts.Data;
using MyWorkouts.Models;
using MyWorkouts.Security;

namespace MyWorkouts.Pages.WorkoutSessions;

public class IndexModel(MyWorkoutsDbContext db) : PageModel
{
    public List<WorkoutSession> Sessions { get; private set; } = [];

    public async Task OnGetAsync()
    {
        var userId = User.GetStableUserId();
        if (string.IsNullOrWhiteSpace(userId)) return;

        Sessions = await db.WorkoutSessions
            .Where(s => s.UserId == userId)
            .OrderByDescending(s => s.StartedAt)
            .ToListAsync();
    }
}
