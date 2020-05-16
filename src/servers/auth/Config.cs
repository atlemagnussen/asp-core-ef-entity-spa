using IdentityModel;
using IdentityServer4.Models;
using System.Collections.Generic;

namespace Test.auth
{
    public class Config
    {
        public const string WebClientName = "webclient";
        public const string MobileClientId = "mobileapp";
        public static IEnumerable<ApiResource> GetApiResources()
        {
            return new List<ApiResource>
            {
                new ApiResource("bankApi", "Customer Api for Bank", new[] { JwtClaimTypes.Email })
                {
                    Scopes = { new Scope("api.read"), new Scope("api.write") }
                }
            };
        }

        public static IEnumerable<IdentityResource> GetIdentityResources()
        {
            return new List<IdentityResource>
            {
                new IdentityResources.OpenId(),
                new IdentityResources.Email(),
                new IdentityResources.Profile(),
                new IdentityResource
                {
                    Name = "roles",
                    DisplayName = "Roles",
                    Description = "Allow the service access to your user roles.",
                    UserClaims = new[] { JwtClaimTypes.Role },
                    ShowInDiscoveryDocument = true,
                    Required = true,
                    Emphasize = true
                }
            };
        }
    }
}
