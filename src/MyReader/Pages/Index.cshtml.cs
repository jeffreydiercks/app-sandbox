using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using MyReader.Data;
using MyReader.Models;
using MyReader.Security;

namespace MyReader.Pages;

public class IndexModel(MyReaderDbContext db) : PageModel
{
    public List<Feed> Feeds { get; private set; } = [];

    public async Task<IActionResult> OnGetAsync()
    {
        var userId = User.GetStableUserId();
        if (string.IsNullOrWhiteSpace(userId))
            return Forbid();

        Feeds = await db.Feeds
            .Where(f => f.UserId == userId)
            .OrderBy(f => f.Title)
            .ToListAsync();

        return Page();
    }
}
