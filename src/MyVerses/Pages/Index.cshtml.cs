using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using MyVerses.Data;
using MyVerses.Models;
using MyVerses.Security;

namespace MyVerses.Pages;

public class IndexModel(MyVersesDbContext db) : PageModel
{
    public int QuoteCount { get; private set; }
    public int ExcerptCount { get; private set; }
    public int PrayerCount { get; private set; }
    public List<Verse> RecentVerses { get; private set; } = [];

    public async Task<IActionResult> OnGetAsync()
    {
        var userId = User.GetStableUserId();
        if (string.IsNullOrWhiteSpace(userId))
            return Forbid();

        var query = db.Verses.Where(v => v.UserId == userId);

        QuoteCount = await query.CountAsync(v => v.Category == VerseCategory.Quote);
        ExcerptCount = await query.CountAsync(v => v.Category == VerseCategory.Excerpt);
        PrayerCount = await query.CountAsync(v => v.Category == VerseCategory.Prayer);
        RecentVerses = await query
            .OrderByDescending(v => v.CreatedAt)
            .Take(5)
            .ToListAsync();

        return Page();
    }
}
