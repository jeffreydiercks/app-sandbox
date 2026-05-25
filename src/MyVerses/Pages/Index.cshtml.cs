using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using MyVerses.Data;
using MyVerses.Models;
using MyVerses.Security;

namespace MyVerses.Pages;

public class IndexModel(MyVersesDbContext db) : PageModel
{
    public Dictionary<VerseCategory, List<Verse>> VersesByCategory { get; private set; } = [];

    public async Task<IActionResult> OnGetAsync()
    {
        var userId = User.GetStableUserId();
        if (string.IsNullOrWhiteSpace(userId))
            return Forbid();

        var verses = await db.Verses
            .Where(v => v.UserId == userId)
            .OrderByDescending(v => v.CreatedAt)
            .ToListAsync();

        VersesByCategory = verses
            .GroupBy(v => v.Category)
            .ToDictionary(g => g.Key, g => g.ToList());

        return Page();
    }
}
