using IdentityServer4;
using IdentityServer4.Models;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;

namespace Test.auth.Services
{
    public interface IHardCodedClientsService
    {
        IEnumerable<Client> GetAll();
        Client Get(string clientId);
    }

    public class HardCodedClientsService : IHardCodedClientsService
    {
        private readonly IConfiguration _configuration;
        public HardCodedClientsService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public Client Get(string clientId)
        {
            switch (clientId)
            {
                case Config.WebClientName:
                    return GetWebClient();
                case Config.MobileClientId:
                    return GetMobileClient();
                default:
                    return GetDynamic(clientId);
            }
        }

        private Client GetDynamic(string clientId)
        {
            var client = new Client
            {
                ClientId = clientId,
                ClientName = "generic client",
                AllowedGrantTypes = GrantTypes.ClientCredentials,
                ClientSecrets = new List<Secret>
                    {
                        new Secret
                        {
                            Type = IdentityServerConstants.SecretTypes.SharedSecret,
                            Description = "shared secret",
                            Value = "sA+iS2dBLR5brbAaO2+oM5cER8XaBQrW3rh6E+ISVmE=".Sha512()
                        },
                        new Secret
                        {
                            Type = IdentityServerConstants.SecretTypes.JsonWebKey,
                            Description = "json web key"
                        }
                    },
                AllowedScopes = { "bankApi" }
            };
            return client;
        }

        private string GenerateNewSecret(int length)
        {
            var crypto = new RNGCryptoServiceProvider();
            byte[] buffer = new byte[length];
            crypto.GetBytes(buffer);
            string uniq = Convert.ToBase64String(buffer);
            return uniq;
        }

        public IEnumerable<Client> GetAll()
        {
            return new List<Client>
            {
                GetWebClient(),
                GetMobileClient()
            };
        }

        private Client GetWebClient()
        {
            string allowedClientUrl = _configuration.GetValue<string>("WebClientUrl");
            var allowedUrls = allowedClientUrl.Split(';').ToList();
            allowedUrls.Add("http://localhost:8080");
            var redirects = new List<string>();
            foreach (var url in allowedUrls)
            {
                redirects.Add(url);
                redirects.Add($"{url}/callback.html");
                redirects.Add($"{url}/silent-renew.html");
            }
            return new Client
            {
                ClientId = Config.WebClientName,
                ClientName = "SPA web client",

                AllowedGrantTypes = GrantTypes.Code,
                RequireClientSecret = false,
                RequirePkce = true,
                RequireConsent = false,
                //RefreshTokenUsage = TokenUsage.OneTimeOnly,
                //UpdateAccessTokenClaimsOnRefresh = true,
                // RefreshTokenExpiration = TokenExpiration.Sliding,

                AccessTokenLifetime = 20 * 60, //24 * 3600,

                RedirectUris = redirects,
                PostLogoutRedirectUris = allowedUrls,
                AllowedCorsOrigins = allowedUrls,

                AllowedScopes =
                    {
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        IdentityServerConstants.StandardScopes.Email,
                        "roles",
                        "api.read",
                        "api.write"
                    },
                AllowOfflineAccess = false
            };
        }
        private Client GetMobileClient()
        {
            return new Client
            {
                ClientId = Config.MobileClientId,
                ClientName = "DigiLEAN mobile app",

                RedirectUris = { "com.companyname.mobileapp://callback" },
                PostLogoutRedirectUris = { "com.companyname.mobileapp://callback" },
                BackChannelLogoutUri = "com.companyname.mobileapp://callback",
                BackChannelLogoutSessionRequired = true,

                RequireClientSecret = false,
                RequireConsent = false,

                AllowedGrantTypes = GrantTypes.Code,
                RequirePkce = true,
                AllowedScopes = { "openid", "profile", "email", "roles", "api.read" },

                AllowOfflineAccess = true,
                RefreshTokenUsage = TokenUsage.OneTimeOnly,
                UpdateAccessTokenClaimsOnRefresh = true,
                RefreshTokenExpiration = TokenExpiration.Sliding,
                SlidingRefreshTokenLifetime = 14 * 24 * 60 * 60
            };
        }
    }
}
