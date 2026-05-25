using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MyGuitar.Data;
using MyGuitar.Models;
using MyGuitar.Security;

namespace MyGuitar.Pages.Lessons;

public class CreateModel(MyGuitarDbContext db) : PageModel
{
    [BindProperty]
    public Lesson Lesson { get; set; } = new();

    public IActionResult OnGet() => Page();

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid) return Page();

        var userId = User.GetStableUserId();
        if (string.IsNullOrWhiteSpace(userId))
            return Forbid();

        Lesson.UserId = userId;
        Lesson.Id = Guid.NewGuid().ToString();
        Lesson.CreatedAt = Lesson.UpdatedAt = DateTime.UtcNow;

        db.Lessons.Add(Lesson);
        await db.SaveChangesAsync();

        return RedirectToPage("Index");
    }
}
