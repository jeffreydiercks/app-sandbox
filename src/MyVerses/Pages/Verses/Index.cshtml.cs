using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Web;
using MyVerses.Data;
using MyVerses.Models;

namespace MyVerses.Pages.Verses;

public class IndexModel(MyVersesDbContext db) : PageModel
{
    public List<Verse> Verses { get; private set; } = [];
    public VerseCategory? CategoryFilter { get; private set; }

    public async Task OnGetAsync(VerseCategory? category)
    {
        CategoryFilter = category;
        var userId = User.GetObjectId() ?? string.Empty;

        var query = db.Verses.Where(v => v.UserId == userId);

        if (category.HasValue)
            query = query.Where(v => v.Category == category.Value);

        Verses = await query
            .OrderByDescending(v => v.CreatedAt)
            .ToListAsync();
    }
}
