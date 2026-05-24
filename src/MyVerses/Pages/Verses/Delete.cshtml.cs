using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Web;
using MyVerses.Data;
using MyVerses.Models;

namespace MyVerses.Pages.Verses;

public class DeleteModel(MyVersesDbContext db) : PageModel
{
    [BindProperty]
    public Verse Verse { get; set; } = null!;

    public async Task<IActionResult> OnGetAsync(string id)
    {
        var userId = User.GetObjectId() ?? string.Empty;
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
        var userId = User.GetObjectId() ?? string.Empty;
        var verse = await db.Verses
            .Where(v => v.UserId == userId && v.Id == Verse.Id)
            .FirstOrDefaultAsync();

        if (verse is null)
            return NotFound();

        db.Verses.Remove(verse);
        await db.SaveChangesAsync();

        return RedirectToPage("Index");
    }
}
