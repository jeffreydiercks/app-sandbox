using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using MyGuitar.Data;
using MyGuitar.Models;
using MyGuitar.Security;

namespace MyGuitar.Pages;

public class IndexModel(MyGuitarDbContext db) : PageModel
{
    public int TotalLessons { get; private set; }
    public int InProgressCount { get; private set; }
    public int CompletedCount { get; private set; }
    public List<Lesson> RecentLessons { get; private set; } = [];

    public async Task<IActionResult> OnGetAsync()
    {
        var userId = User.GetStableUserId();
        if (string.IsNullOrWhiteSpace(userId))
            return Forbid();

        var lessons = await db.Lessons
            .Where(l => l.UserId == userId)
            .OrderByDescending(l => l.LastPracticedAt ?? l.UpdatedAt)
            .ToListAsync();

        TotalLessons = lessons.Count;
        InProgressCount = lessons.Count(l => l.Status == LessonStatus.InProgress);
        CompletedCount = lessons.Count(l => l.Status == LessonStatus.Completed);
        RecentLessons = lessons.Take(5).ToList();

        return Page();
    }
}
