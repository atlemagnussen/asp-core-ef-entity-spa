using IdentityModel;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;

namespace mobileapp.Core.Services
{
    public static class ClaimsHelper
    {
        private static string[] firstNameClaimTypes = new string[] { JwtClaimTypes.GivenName, ClaimTypes.GivenName };
        private static string[] lastNameClaimTypes = new string[] { JwtClaimTypes.FamilyName, ClaimTypes.Surname };
        private static string[] emailClaimTypes = new string[] { JwtClaimTypes.Email, ClaimTypes.Email, JwtClaimTypes.PreferredUserName };
        private static string[] userNameClaimTypes = new string[] { JwtClaimTypes.PreferredUserName, JwtClaimTypes.Name, JwtClaimTypes.Email };

        public static string GetFullName(IEnumerable<Claim> claims)
        {
            var fullName = GetNameByFullName(claims);
            if (string.IsNullOrEmpty(fullName))
                return GetNameByFirstAndLastName(claims);

            return fullName;
        }

        public static string GetUserName(IEnumerable<Claim> claims)
        {
            var userNameClaim = TryClaims(claims, userNameClaimTypes);
            if (userNameClaim != null)
                return userNameClaim.Value;
            return null;
        }
        public static string GetEmail(IEnumerable<Claim> claims)
        {
            var emailClaim = TryClaims(claims, emailClaimTypes);
            if (emailClaim != null)
                return emailClaim.Value;
            return null;
        }

        private static string GetNameByFullName(IEnumerable<Claim> claims)
        {
            var nameClaim = claims.FirstOrDefault(x => x.Type == JwtClaimTypes.Name) ??
                claims.FirstOrDefault(x => x.Type == ClaimTypes.Name);
            if (nameClaim != null)
                return nameClaim.Value;

            return null;
        }
        private static string GetNameByFirstAndLastName(IEnumerable<Claim> claims)
        {
            var firstClaim = TryClaims(claims, firstNameClaimTypes);
            var lastClaim = TryClaims(claims, lastNameClaimTypes);

            if (firstClaim != null && lastClaim != null)
            {
                return $"{firstClaim.Value} {lastClaim.Value}";
            }
            else if (firstClaim != null)
            {
                return firstClaim.Value;
            }
            else if (lastClaim != null)
            {
                return lastClaim.Value;
            }
            return null;
        }

        private static Claim TryClaims(IEnumerable<Claim> claims, IEnumerable<string> tryClaimNames)
        {
            foreach (var claimName in tryClaimNames)
            {
                var claim = claims.FirstOrDefault(x => x.Type == claimName);
                if (claim != null)
                    return claim;
            }
            return null;
        }
    }
}
