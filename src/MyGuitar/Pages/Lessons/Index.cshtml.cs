using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using MyGuitar.Data;
using MyGuitar.Models;
using MyGuitar.Security;

namespace MyGuitar.Pages.Lessons;

public class IndexModel(MyGuitarDbContext db) : PageModel
{
    public List<Lesson> Lessons { get; private set; } = [];

    public async Task<IActionResult> OnGetAsync()
    {
        var userId = User.GetStableUserId();
        if (string.IsNullOrWhiteSpace(userId))
            return Forbid();

        Lessons = await db.Lessons
            .Where(l => l.UserId == userId)
            .OrderBy(l => l.Stage)
            .ThenBy(l => l.Title)
            .ToListAsync();

        return Page();
    }
}
