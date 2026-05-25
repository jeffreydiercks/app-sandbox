using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MyGuitar.Data;
using MyGuitar.Models;
using MyGuitar.Security;

namespace MyGuitar.Pages.Lessons;

public class DetailsModel(MyGuitarDbContext db) : PageModel
{
    public Lesson? Lesson { get; private set; }

    public async Task<IActionResult> OnGetAsync(string id)
    {
        var userId = User.GetStableUserId();
        if (string.IsNullOrWhiteSpace(userId))
            return Forbid();

        Lesson = await db.Lessons.FindAsync(id, userId);
        return Page();
    }

    public async Task<IActionResult> OnPostMarkPracticedAsync(string id)
    {
        var userId = User.GetStableUserId();
        if (string.IsNullOrWhiteSpace(userId))
            return Forbid();

        var lesson = await db.Lessons.FindAsync(id, userId);
        if (lesson is not null)
        {
            lesson.LastPracticedAt = DateTime.UtcNow;
            lesson.UpdatedAt = DateTime.UtcNow;
            if (lesson.Status == LessonStatus.NotStarted)
                lesson.Status = LessonStatus.InProgress;
            await db.SaveChangesAsync();
        }

        return RedirectToPage(new { id });
    }
}
