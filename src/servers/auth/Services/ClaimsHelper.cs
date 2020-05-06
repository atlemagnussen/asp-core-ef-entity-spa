using IdentityModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;

namespace Test.auth.Services
{
    public interface IClaimsHelper
    {
        string GetFullName(IEnumerable<Claim> claims);
        string GetEmail(IEnumerable<Claim> claims);
    }

    public class ClaimsHelper : IClaimsHelper
    {
        private string[] firstNameClaimTypes = new string[] { JwtClaimTypes.GivenName, ClaimTypes.GivenName };
        private string[] lastNameClaimTypes = new string[] { JwtClaimTypes.FamilyName, ClaimTypes.Surname };
        private string[] emailClaimTypes = new string[] { JwtClaimTypes.Email, ClaimTypes.Email, JwtClaimTypes.PreferredUserName };

        public string GetFullName(IEnumerable<Claim> claims)
        {
            var fullName = GetNameByFullName(claims);
            if (string.IsNullOrEmpty(fullName))
                return GetNameByFirstAndLastName(claims);
            
            return fullName;
        }

        public string GetEmail(IEnumerable<Claim> claims)
        {
            var emailClaim = TryClaims(claims, emailClaimTypes);
            if (emailClaim != null)
                return emailClaim.Value;
            return null;
        }

        private string GetNameByFullName(IEnumerable<Claim> claims)
        {
            var nameClaim = claims.FirstOrDefault(x => x.Type == JwtClaimTypes.Name) ??
                claims.FirstOrDefault(x => x.Type == ClaimTypes.Name);
            if (nameClaim != null)
               return nameClaim.Value;
            
            return null;
        }
        private string GetNameByFirstAndLastName(IEnumerable<Claim> claims)
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

        private Claim TryClaims(IEnumerable<Claim> claims, IEnumerable<string> tryClaimNames)
        {
            foreach(var claimName in tryClaimNames)
            {
                var claim = claims.FirstOrDefault(x => x.Type == claimName);
                if (claim != null)
                    return claim;
            }
            return null;
        }
    }
}
