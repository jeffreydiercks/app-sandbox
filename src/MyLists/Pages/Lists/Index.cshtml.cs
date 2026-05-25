using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using MyLists.Data;
using MyLists.Models;
using MyLists.Security;

namespace MyLists.Pages.Lists;

public class IndexModel(MyListsDbContext db) : PageModel
{
    public List<(TaskList List, int OpenCount)> Lists { get; private set; } = [];

    public async Task<IActionResult> OnGetAsync()
    {
        var userId = User.GetStableUserId();
        if (string.IsNullOrWhiteSpace(userId))
            return Forbid();

        var lists = await db.Lists
            .Where(l => l.UserId == userId)
            .OrderBy(l => l.Name)
            .ToListAsync();

        var items = await db.Items
            .Where(i => i.UserId == userId && !i.IsCompleted)
            .ToListAsync();

        Lists = lists
            .Select(l => (l, items.Count(i => i.ListId == l.Id)))
            .ToList();

        return Page();
    }
}
