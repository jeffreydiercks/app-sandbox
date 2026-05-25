using System.Security.Claims;
using Microsoft.Identity.Web;

namespace MyGuitar.Security;

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
