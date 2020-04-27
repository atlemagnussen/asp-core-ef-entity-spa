using Azure.Identity;
using Azure.Security.KeyVault.Keys.Cryptography;
using IdentityServer4.Configuration;
using IdentityServer4.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using System.Threading.Tasks;

namespace Test.auth.Services
{
    public class AzureKeyVaultTokenCreationService : DefaultTokenCreationService
    {
        private string _vaultUrl;
        private string _signingKeyName;
        private string _keyUrl;
        private ILogger<AzureKeyVaultTokenCreationService> _logger;
        private CryptographyClient _cryptoClient;
        public AzureKeyVaultTokenCreationService(IConfiguration configuration,
            ISystemClock clock, 
            IKeyMaterialService keys, 
            IdentityServerOptions options, 
            ILogger<DefaultTokenCreationService> logger,
            ILogger<AzureKeyVaultTokenCreationService> loggerHere,
            IWebHostEnvironment environment)
            : base(clock, keys, options, logger)
        {
            _logger = loggerHere;
            _signingKeyName = configuration.GetValue<string>("SigningKeyName");
            _vaultUrl = $"https://{configuration["KeyVaultName"]}.vault.azure.net/";
            _keyUrl = $"{_vaultUrl}keys/{_signingKeyName}";

            if (environment.IsDevelopment())
            {
                var clientId = configuration.GetValue<string>("AzureKeyVault:clientId");
                var tenantId = configuration.GetValue<string>("AzureKeyVault:tenantId");
                var clientSecret = configuration.GetValue<string>("AzureKeyVault:clientSecret");
                var clientCredential = new ClientSecretCredential(tenantId, clientId, clientSecret);
                _cryptoClient = new CryptographyClient(new Uri(_keyUrl), clientCredential);
            }
            else
            {
                _cryptoClient = new CryptographyClient(new Uri(_keyUrl), new DefaultAzureCredential());
            }
            
        }

        protected override async Task<string> CreateJwtAsync(JwtSecurityToken jwt)
        {
            _logger.LogInformation("AzureKeyVaultTokenCreationService");
            _logger.LogInformation($"_keyUrl= {_keyUrl}");
            var plaintext = $"{jwt.EncodedHeader}.{jwt.EncodedPayload}";

            byte[] hash;
            using (var hasher = CryptoHelper.GetHashAlgorithmForSigningAlgorithm(jwt.SignatureAlgorithm))
                hash = hasher.ComputeHash(Encoding.UTF8.GetBytes(plaintext));

            var signResult = await _cryptoClient.SignAsync(new SignatureAlgorithm(jwt.SignatureAlgorithm), hash);

            return $"{plaintext}.{Base64UrlTextEncoder.Encode(signResult.Signature)}";
        }
    }
}
