using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Identity.Web;
using MyVerses.Data;
using MyVerses.Models;

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
        var userId =
            User.GetObjectId()
            ?? User.GetHomeObjectId()
            ?? System.Security.Claims.ClaimsPrincipalExtensions.FindFirstValue(User, "sub")
            ?? System.Security.Claims.ClaimsPrincipalExtensions.FindFirstValue(User, System.Security.Claims.ClaimTypes.NameIdentifier);

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
