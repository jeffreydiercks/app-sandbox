using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using MyVerses.Data;
using MyVerses.Models;
using MyVerses.Security;

namespace MyVerses.Pages.Verses;

public class IndexModel(MyVersesDbContext db) : PageModel
{
    public List<Verse> Verses { get; private set; } = [];
    public VerseCategory? CategoryFilter { get; private set; }

    public async Task<Microsoft.AspNetCore.Mvc.IActionResult> OnGetAsync(VerseCategory? category)
    {
        CategoryFilter = category;

        var userId = User.GetStableUserId();

        if (string.IsNullOrWhiteSpace(userId))
            return Forbid();

        var query = db.Verses.Where(v => v.UserId == userId);

        if (category.HasValue)
            query = query.Where(v => v.Category == category.Value);

        Verses = await query
            .OrderByDescending(v => v.CreatedAt)
            .ToListAsync();

        return Page();
    }
}
