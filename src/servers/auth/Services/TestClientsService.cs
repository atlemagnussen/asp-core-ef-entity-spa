using IdentityServer4;
using IdentityServer4.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Test.dataaccess;
using Test.model;

namespace Test.auth.Services
{
    public interface ITestClientsService
    {
        IEnumerable<Client> GetAll();
        Task<string> Create(string clientId);
        Task<Client> Get(string clientId);
    }

    public class TestClientsService : ITestClientsService
    {
        private readonly IConfiguration _configuration;
        private readonly AuthDbContext _authContext;
        public TestClientsService(IConfiguration configuration,
            AuthDbContext authContext)
        {
            _configuration = configuration;
            _authContext = authContext;
        }

        public async Task<Client> Get(string clientId)
        {
            switch (clientId)
            {
                case Config.WebClientName:
                    return GetWebClient();
                case Config.MobileClientId:
                    return GetMobileClient();
                default:
                    return await GetDynamic(clientId);
            }
        }

        public async Task<string> Create(string clientId)
        {
            clientId = clientId.Trim();
            if (clientId == Config.WebClientName || clientId == Config.WebClientName)
                throw new ApplicationException("Client name is taken");

            if (await _authContext.ApiClients.AnyAsync(a => a.ClientId == clientId))
                throw new ApplicationException("Client name is taken");

            var secret = GenerateNewSecret(32);
            var client = new ApiClient
            {
                ClientId = clientId,
                Name = "Something",
                SecretType = ClientSecretType.SharedSecret,
                Secret = secret.Sha512()
            };
            await _authContext.ApiClients.AddAsync(client);
            await _authContext.SaveChangesAsync();
            return secret;
        }

        private async Task<Client> GetDynamic(string clientId)
        {
            var db = await _authContext.ApiClients.FirstOrDefaultAsync(a => a.ClientId == clientId);
            if (db == null)
                return null;

            var client = new Client
            {
                ClientId = db.ClientId,
                ClientName = db.Name,
                AllowedGrantTypes = GrantTypes.ClientCredentials,
                ClientSecrets = new List<Secret>(),
                AllowedScopes = { "bankApi" }
            };
            if (db.SecretType == ClientSecretType.SharedSecret)
            {
                client.ClientSecrets.Add(new Secret
                {
                    Type = IdentityServerConstants.SecretTypes.SharedSecret,
                    Description = "shared secret",
                    Value = db.Secret
                });
            }
            else if (db.SecretType == ClientSecretType.JsonWebKey)
            {
                client.ClientSecrets.Add(new Secret
                {
                    Type = IdentityServerConstants.SecretTypes.JsonWebKey,
                    Description = "json web key",
                    Value = db.Secret
                });
            }
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

                AccessTokenLifetime = 24 * 3600,

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
