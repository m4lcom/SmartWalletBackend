using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;

namespace SmartWallet.Infrastructure.Extensions
{
    public static class ClaimsPrincipalExtensions
    {


        public static Guid? GetUserId(this ClaimsPrincipal user)
        {
            if (user == null) return null;

            var sub = user.FindFirst(JwtRegisteredClaimNames.Sub)?.Value
                      ?? user.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            return Guid.TryParse(sub, out var id) ? id : null;
        }

        public static string? GetUserEmail(this ClaimsPrincipal user)
        {
            if (user == null) return null;

            return user.FindFirst(JwtRegisteredClaimNames.Email)?.Value
                   ?? user.FindFirst(ClaimTypes.Email)?.Value;
        }

        public static bool IsAdmin(this ClaimsPrincipal user)
        {
            if (user == null) return false;

            if (user.IsInRole("Admin")) return true;

            var roleClaim = user.FindFirst("role")?.Value;
            return string.Equals(roleClaim, "Admin", StringComparison.OrdinalIgnoreCase);
        }
    }
}