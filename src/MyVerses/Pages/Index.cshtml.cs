using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Web;
using MyVerses.Data;
using MyVerses.Models;

namespace MyVerses.Pages;

public class IndexModel(MyVersesDbContext db) : PageModel
{
    public int QuoteCount { get; private set; }
    public int ExcerptCount { get; private set; }
    public int PrayerCount { get; private set; }
    public List<Verse> RecentVerses { get; private set; } = [];

    public async Task OnGetAsync()
    {
        var userId = User.GetObjectId() ?? string.Empty;

        var verses = await db.Verses
            .Where(v => v.UserId == userId)
            .ToListAsync();

        QuoteCount = verses.Count(v => v.Category == VerseCategory.Quote);
        ExcerptCount = verses.Count(v => v.Category == VerseCategory.Excerpt);
        PrayerCount = verses.Count(v => v.Category == VerseCategory.Prayer);
        RecentVerses = verses.OrderByDescending(v => v.CreatedAt).Take(5).ToList();
    }
}
