using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using MyLists.Data;
using MyLists.Models;
using MyLists.Security;

namespace MyLists.Pages.Lists;

public class DeleteModel(MyListsDbContext db) : PageModel
{
    public TaskList? List { get; private set; }

    public async Task<IActionResult> OnGetAsync(string id)
    {
        var userId = User.GetStableUserId();
        if (string.IsNullOrWhiteSpace(userId))
            return Forbid();

        List = await db.Lists.FindAsync(id, userId);
        return Page();
    }

    public async Task<IActionResult> OnPostAsync(string id)
    {
        var userId = User.GetStableUserId();
        if (string.IsNullOrWhiteSpace(userId))
            return Forbid();

        var list = await db.Lists.FindAsync(id, userId);
        if (list is not null)
        {
            var items = await db.Items
                .Where(i => i.UserId == userId && i.ListId == id)
                .ToListAsync();
            db.Items.RemoveRange(items);
            db.Lists.Remove(list);
            await db.SaveChangesAsync();
        }

        return RedirectToPage("Index");
    }
}
