using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Web;
using MyVerses.Data;
using MyVerses.Models;

namespace MyVerses.Pages.Verses;

public class DetailsModel(MyVersesDbContext db) : PageModel
{
    public Verse Verse { get; private set; } = null!;

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
}
