using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MyGuitar.Data;
using MyGuitar.Models;
using MyGuitar.Security;

namespace MyGuitar.Pages.Lessons;

public class EditModel(MyGuitarDbContext db) : PageModel
{
    [BindProperty]
    public Lesson? Lesson { get; set; }

    public async Task<IActionResult> OnGetAsync(string id)
    {
        var userId = User.GetStableUserId();
        if (string.IsNullOrWhiteSpace(userId))
            return Forbid();

        Lesson = await db.Lessons.FindAsync(id, userId);
        if (Lesson is null)
            return NotFound();

        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid) return Page();

        var userId = User.GetStableUserId();
        if (string.IsNullOrWhiteSpace(userId))
            return Forbid();

        var existing = await db.Lessons.FindAsync(Lesson!.Id, userId);
        if (existing is null)
            return NotFound();

        existing.Title = Lesson.Title;
        existing.Stage = Lesson.Stage;
        existing.Status = Lesson.Status;
        existing.Notes = Lesson.Notes;
        existing.LastPracticedAt = Lesson.LastPracticedAt;
        existing.UpdatedAt = DateTime.UtcNow;

        await db.SaveChangesAsync();
        return RedirectToPage("Details", new { id = existing.Id });
    }
}
