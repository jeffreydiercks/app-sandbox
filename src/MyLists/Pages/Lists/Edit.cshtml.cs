using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MyLists.Data;
using MyLists.Models;
using MyLists.Security;

namespace MyLists.Pages.Lists;

public class EditModel(MyListsDbContext db) : PageModel
{
    [BindProperty]
    public TaskList? List { get; set; }

    public async Task<IActionResult> OnGetAsync(string id)
    {
        var userId = User.GetStableUserId();
        if (string.IsNullOrWhiteSpace(userId))
            return Forbid();

        List = await db.Lists.FindAsync(id, userId);
        if (List is null)
            return NotFound();

        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid) return Page();

        var userId = User.GetStableUserId();
        if (string.IsNullOrWhiteSpace(userId))
            return Forbid();

        var existing = await db.Lists.FindAsync(List!.Id, userId);
        if (existing is null)
            return NotFound();

        existing.Name = List.Name;
        existing.UpdatedAt = DateTime.UtcNow;

        await db.SaveChangesAsync();
        return RedirectToPage("Index");
    }
}
