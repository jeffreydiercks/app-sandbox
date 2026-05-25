using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MyLists.Data;
using MyLists.Models;
using MyLists.Security;

namespace MyLists.Pages.Lists.Items;

public class CreateModel(MyListsDbContext db) : PageModel
{
    [BindProperty]
    public TaskItem Item { get; set; } = new();

    public IActionResult OnGet(string listId)
    {
        Item.ListId = listId;
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid) return Page();

        var userId = User.GetStableUserId();
        if (string.IsNullOrWhiteSpace(userId))
            return Forbid();

        Item.Id = Guid.NewGuid().ToString();
        Item.UserId = userId;
        Item.CreatedAt = Item.UpdatedAt = DateTime.UtcNow;

        db.Items.Add(Item);
        await db.SaveChangesAsync();

        return RedirectToPage("/Lists/View", new { id = Item.ListId });
    }
}
