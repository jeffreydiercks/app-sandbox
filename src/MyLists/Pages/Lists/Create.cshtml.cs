using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MyLists.Data;
using MyLists.Models;
using MyLists.Security;

namespace MyLists.Pages.Lists;

public class CreateModel(MyListsDbContext db) : PageModel
{
    [BindProperty]
    public TaskList List { get; set; } = new();

    public IActionResult OnGet() => Page();

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid) return Page();

        var userId = User.GetStableUserId();
        if (string.IsNullOrWhiteSpace(userId))
            return Forbid();

        List.Id = Guid.NewGuid().ToString();
        List.UserId = userId;
        List.CreatedAt = List.UpdatedAt = DateTime.UtcNow;

        db.Lists.Add(List);
        await db.SaveChangesAsync();

        return RedirectToPage("Index");
    }
}
