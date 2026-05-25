using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using MyReader.Data;
using MyReader.Models;
using MyReader.Security;
using MyReader.Services;

namespace MyReader.Pages.Feeds;

public class ViewModel(MyReaderDbContext db, RssService rss) : PageModel
{
    public Feed? Feed { get; private set; }
    public List<FeedArticle> Articles { get; private set; } = [];
    public string? ErrorMessage { get; private set; }

    public async Task<IActionResult> OnGetAsync(string id)
    {
        var userId = User.GetStableUserId();
        if (string.IsNullOrWhiteSpace(userId))
            return Forbid();

        Feed = await db.Feeds.FindAsync(id, userId);
        if (Feed is null)
            return NotFound();

        var rawArticles = await rss.ParseFeedAsync(Feed.Url);
        if (rawArticles.Count == 0)
            ErrorMessage = "Could not load articles. The feed may be unavailable or invalid.";

        var readGuids = await db.ReadItems
            .Where(r => r.UserId == userId && r.FeedId == id)
            .Select(r => r.ItemGuid)
            .ToListAsync();

        var readSet = readGuids.ToHashSet();
        foreach (var article in rawArticles)
            article.IsRead = readSet.Contains(article.Guid);

        Articles = rawArticles;
        return Page();
    }

    public async Task<IActionResult> OnPostMarkReadAsync(string feedId, string guid)
    {
        var userId = User.GetStableUserId();
        if (string.IsNullOrWhiteSpace(userId))
            return Forbid();

        var alreadyRead = await db.ReadItems
            .AnyAsync(r => r.UserId == userId && r.FeedId == feedId && r.ItemGuid == guid);

        if (!alreadyRead)
        {
            db.ReadItems.Add(new ReadItem
            {
                Id = Guid.NewGuid().ToString(),
                UserId = userId,
                FeedId = feedId,
                ItemGuid = guid,
                ReadAt = DateTime.UtcNow
            });
            await db.SaveChangesAsync();
        }

        return RedirectToPage(new { id = feedId });
    }
}
