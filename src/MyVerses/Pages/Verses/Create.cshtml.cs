using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MyVerses.Data;
using MyVerses.Models;
using MyVerses.Security;

namespace MyVerses.Pages.Verses;

public class CreateModel(MyVersesDbContext db) : PageModel
{
    [BindProperty]
    public Verse Verse { get; set; } = new();

    public void OnGet() { }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
            return Page();
        var userId = User.GetStableUserId();

        if (string.IsNullOrWhiteSpace(userId))
            return Forbid();

        Verse.Id = Guid.NewGuid().ToString();
        Verse.UserId = userId;
        Verse.CreatedAt = DateTime.UtcNow;
        Verse.UpdatedAt = DateTime.UtcNow;

        db.Verses.Add(Verse);
        await db.SaveChangesAsync();

        return RedirectToPage("Index");
    }
}
