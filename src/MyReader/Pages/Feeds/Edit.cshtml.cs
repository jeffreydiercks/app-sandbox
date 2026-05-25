using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MyReader.Data;
using MyReader.Models;
using MyReader.Security;

namespace MyReader.Pages.Feeds;

public class EditModel(MyReaderDbContext db) : PageModel
{
    [BindProperty]
    public Feed? Feed { get; set; }

    public async Task<IActionResult> OnGetAsync(string id)
    {
        var userId = User.GetStableUserId();
        if (string.IsNullOrWhiteSpace(userId))
            return Forbid();

        Feed = await db.Feeds.FindAsync(id, userId);
        if (Feed is null)
            return NotFound();

        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid) return Page();

        var userId = User.GetStableUserId();
        if (string.IsNullOrWhiteSpace(userId))
            return Forbid();

        var existing = await db.Feeds.FindAsync(Feed!.Id, userId);
        if (existing is null)
            return NotFound();

        existing.Title = Feed.Title;
        existing.Url = Feed.Url;
        existing.UpdatedAt = DateTime.UtcNow;

        await db.SaveChangesAsync();
        return RedirectToPage("Index");
    }
}
