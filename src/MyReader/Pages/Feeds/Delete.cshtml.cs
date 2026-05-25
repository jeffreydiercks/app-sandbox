using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using MyReader.Data;
using MyReader.Models;
using MyReader.Security;

namespace MyReader.Pages.Feeds;

public class DeleteModel(MyReaderDbContext db) : PageModel
{
    public Feed? Feed { get; private set; }

    public async Task<IActionResult> OnGetAsync(string id)
    {
        var userId = User.GetStableUserId();
        if (string.IsNullOrWhiteSpace(userId))
            return Forbid();

        Feed = await db.Feeds.FindAsync(id, userId);
        return Page();
    }

    public async Task<IActionResult> OnPostAsync(string id)
    {
        var userId = User.GetStableUserId();
        if (string.IsNullOrWhiteSpace(userId))
            return Forbid();

        var feed = await db.Feeds.FindAsync(id, userId);
        if (feed is not null)
        {
            var readItems = await db.ReadItems
                .Where(r => r.UserId == userId && r.FeedId == id)
                .ToListAsync();
            db.ReadItems.RemoveRange(readItems);
            db.Feeds.Remove(feed);
            await db.SaveChangesAsync();
        }

        return RedirectToPage("Index");
    }
}
