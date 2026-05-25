using System.Security.Claims;
using Microsoft.Identity.Web;

namespace MyWorkouts.Security;

internal static class UserIdentityExtensions
{
    public static string? GetStableUserId(this ClaimsPrincipal user)
    {
        return user.GetObjectId()
            ?? user.GetHomeObjectId()
            ?? user.FindFirstValue("sub")
            ?? user.FindFirstValue(ClaimTypes.NameIdentifier);
    }
}
