using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MyLists.Data;
using MyLists.Models;
using MyLists.Security;

namespace MyLists.Pages.Lists.Items;

public class EditModel(MyListsDbContext db) : PageModel
{
    [BindProperty]
    public TaskItem? Item { get; set; }

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

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid) return Page();

        var userId = User.GetStableUserId();
        if (string.IsNullOrWhiteSpace(userId))
            return Forbid();

        var existing = await db.Items.FindAsync(Item!.Id, userId);
        if (existing is null)
            return NotFound();

        existing.Title = Item.Title;
        existing.Notes = Item.Notes;
        existing.DueDate = Item.DueDate;
        existing.UpdatedAt = DateTime.UtcNow;

        await db.SaveChangesAsync();
        return RedirectToPage("/Lists/View", new { id = existing.ListId });
    }
}
