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
        private readonly IWebHostEnvironment _environment;
        private readonly IConfiguration _configuration;
        private string _vaultUrl;
        private string _signingKeyName;
        private ILogger<AzureKeyVaultTokenCreationService> _logger;
        private readonly IAzureKeyService _azureKeyService;

        public AzureKeyVaultTokenCreationService(IConfiguration configuration,
            ISystemClock clock, 
            IKeyMaterialService keys, 
            IdentityServerOptions options, 
            ILogger<DefaultTokenCreationService> logger,
            ILogger<AzureKeyVaultTokenCreationService> loggerHere,
            IWebHostEnvironment environment,
            IAzureKeyService azureKeyService)
            : base(clock, keys, options, logger)
        {
            _environment = environment;
            _configuration = configuration;
            _logger = loggerHere;
            _azureKeyService = azureKeyService;

            _signingKeyName = configuration.GetValue<string>("SigningKeyName");
            _vaultUrl = $"https://{configuration["KeyVaultName"]}.vault.azure.net/";
        }

        private async Task<CryptographyClient> GetClient()
        {
            var keys = await _azureKeyService.GetSigningKeysAsync();
            var currentVersion = keys.Current.Version;
            var keyUrl = $"{_vaultUrl}keys/{_signingKeyName}/{currentVersion}";
            _logger.LogInformation($"_keyUrl= {keyUrl}");
            CryptographyClient client;
            if (_environment.IsDevelopment())
            {
                var clientId = _configuration.GetValue<string>("AzureKeyVault:clientId");
                var tenantId = _configuration.GetValue<string>("AzureKeyVault:tenantId");
                var clientSecret = _configuration.GetValue<string>("AzureKeyVault:clientSecret");
                var clientCredential = new ClientSecretCredential(tenantId, clientId, clientSecret);
                client = new CryptographyClient(new Uri(keyUrl), clientCredential);
            }
            else
            {
                client = new CryptographyClient(new Uri(keyUrl), new DefaultAzureCredential());
            }
            return client;
        }

        protected override async Task<string> CreateJwtAsync(JwtSecurityToken jwt)
        {
            _logger.LogInformation("AzureKeyVaultTokenCreationService");
            
            var plaintext = $"{jwt.EncodedHeader}.{jwt.EncodedPayload}";

            byte[] hash;
            using (var hasher = CryptoHelper.GetHashAlgorithmForSigningAlgorithm(jwt.SignatureAlgorithm))
                hash = hasher.ComputeHash(Encoding.UTF8.GetBytes(plaintext));

            var client = await GetClient();
            var signResult = await client.SignAsync(new SignatureAlgorithm(jwt.SignatureAlgorithm), hash);

            return $"{plaintext}.{Base64UrlTextEncoder.Encode(signResult.Signature)}";
        }
    }
}
