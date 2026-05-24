using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Web;
using MyVerses.Data;
using MyVerses.Models;

namespace MyVerses.Pages.Verses;

public class EditModel(MyVersesDbContext db) : PageModel
{
    [BindProperty]
    public Verse Verse { get; set; } = null!;

    public async Task<IActionResult> OnGetAsync(string id)
    {
        var userId =
            User.GetObjectId()
            ?? User.GetHomeObjectId()
            ?? System.Security.Claims.ClaimsPrincipalExtensions.FindFirstValue(User, "sub")
            ?? System.Security.Claims.ClaimsPrincipalExtensions.FindFirstValue(User, System.Security.Claims.ClaimTypes.NameIdentifier);

        if (string.IsNullOrWhiteSpace(userId))
            return Forbid();

        var verse = await db.Verses
            .Where(v => v.UserId == userId && v.Id == id)
            .FirstOrDefaultAsync();

        if (verse is null)
            return NotFound();

        Verse = verse;
        return Page();
    }

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

        var verse = await db.Verses
            .Where(v => v.UserId == userId && v.Id == Verse.Id)
            .FirstOrDefaultAsync();

        if (verse is null)
            return NotFound();

        verse.Title = Verse.Title;
        verse.Content = Verse.Content;
        verse.Author = Verse.Author;
        verse.Category = Verse.Category;
        verse.UpdatedAt = DateTime.UtcNow;

        await db.SaveChangesAsync();

        return RedirectToPage("Details", new { id = verse.Id });
    }
}
