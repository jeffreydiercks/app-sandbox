using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MyLists.Data;
using MyLists.Models;
using MyLists.Security;

namespace MyLists.Pages.Lists.Items;

public class DeleteModel(MyListsDbContext db) : PageModel
{
    public TaskItem? Item { get; private set; }

    public async Task<IActionResult> OnGetAsync(string id, string listId)
    {
        var userId = User.GetStableUserId();
        if (string.IsNullOrWhiteSpace(userId))
            return Forbid();

        Item = await db.Items.FindAsync(id, userId);
        if (Item is null || Item.ListId != listId)
            return NotFound();

        return Page();
    }

    public async Task<IActionResult> OnPostAsync(string id, string listId)
    {
        var userId = User.GetStableUserId();
        if (string.IsNullOrWhiteSpace(userId))
            return Forbid();

        var item = await db.Items.FindAsync(id, userId);
        if (item is not null)
        {
            db.Items.Remove(item);
            await db.SaveChangesAsync();
        }

        return RedirectToPage("/Lists/View", new { id = listId });
    }
}
