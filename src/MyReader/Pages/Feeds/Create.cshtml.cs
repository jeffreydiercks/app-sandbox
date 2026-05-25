using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MyReader.Data;
using MyReader.Models;
using MyReader.Security;

namespace MyReader.Pages.Feeds;

public class CreateModel(MyReaderDbContext db) : PageModel
{
    [BindProperty]
    public Feed Feed { get; set; } = new();

    public IActionResult OnGet() => Page();

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid) return Page();

        var userId = User.GetStableUserId();
        if (string.IsNullOrWhiteSpace(userId))
            return Forbid();

        Feed.Id = Guid.NewGuid().ToString();
        Feed.UserId = userId;
        Feed.CreatedAt = Feed.UpdatedAt = DateTime.UtcNow;

        db.Feeds.Add(Feed);
        await db.SaveChangesAsync();

        return RedirectToPage("Index");
    }
}
