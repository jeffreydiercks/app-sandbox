using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using MyLists.Data;
using MyLists.Models;
using MyLists.Security;

namespace MyLists.Pages.Lists;

public class ViewModel(MyListsDbContext db) : PageModel
{
    public TaskList? List { get; private set; }
    public List<TaskItem> Items { get; private set; } = [];

    public async Task<IActionResult> OnGetAsync(string id)
    {
        var userId = User.GetStableUserId();
        if (string.IsNullOrWhiteSpace(userId))
            return Forbid();

        List = await db.Lists.FindAsync(id, userId);
        if (List is null)
            return NotFound();

        Items = await db.Items
            .Where(i => i.UserId == userId && i.ListId == id)
            .ToListAsync();

        return Page();
    }

    public async Task<IActionResult> OnPostToggleAsync(string itemId, string listId)
    {
        var userId = User.GetStableUserId();
        if (string.IsNullOrWhiteSpace(userId))
            return Forbid();

        var item = await db.Items.FindAsync(itemId, userId);
        if (item is not null)
        {
            item.IsCompleted = !item.IsCompleted;
            item.CompletedAt = item.IsCompleted ? DateTime.UtcNow : null;
            item.UpdatedAt = DateTime.UtcNow;
            await db.SaveChangesAsync();
        }

        return RedirectToPage(new { id = listId });
    }
}
