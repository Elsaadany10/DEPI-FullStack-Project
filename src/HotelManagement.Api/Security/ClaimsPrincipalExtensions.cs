using System.Security.Claims;

namespace HotelManagement.Api.Security;

public static class ClaimsPrincipalExtensions
{
    public static CurrentUser? ToCurrentUser(this ClaimsPrincipal principal)
    {
        var idValue = principal.FindFirstValue(ClaimTypes.NameIdentifier)
                      ?? principal.FindFirstValue("sub");

        if (!int.TryParse(idValue, out var userId))
        {
            return null;
        }

        var email = principal.FindFirstValue(ClaimTypes.Email)
                    ?? principal.FindFirstValue("email")
                    ?? string.Empty;

        var role = principal.FindFirstValue(ClaimTypes.Role)
                   ?? principal.FindFirstValue("role")
                   ?? string.Empty;

        return new CurrentUser(userId, email, role);
    }
}