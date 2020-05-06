using IdentityModel;
using System.Security.Claims;
using System.Security.Principal;

namespace Test.model
{
    public static class IdentityExtensions
    {
        public static string GetUserIdFromClaim(this IIdentity identity)
        {
            var claim = ((ClaimsIdentity)identity).FindFirst(JwtClaimTypes.Subject);
            if (claim == null) return "";
            return claim.Value;
        }
    }
}
