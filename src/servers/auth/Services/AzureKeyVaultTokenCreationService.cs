using Azure.Identity;
using Azure.Security.KeyVault.Keys.Cryptography;
using IdentityServer4.Configuration;
using IdentityServer4.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using System.Threading.Tasks;
using Test.dataaccess;
using Test.dataaccess.Services;

namespace Test.auth.Services
{
    /// <summary>
    /// Get Signing Keys
    /// </summary>
    public class AzureKeyVaultTokenCreationService : DefaultTokenCreationService
    {
        private readonly SettingsAzureKeyVault _settings;
        private readonly IWebHostEnvironment _environment;
        private string _vaultUrl;
        private string _signingKeyName;
        private ILogger<AzureKeyVaultTokenCreationService> _logger;
        private readonly IAzureKeyService _azureKeyService;

        public AzureKeyVaultTokenCreationService(IOptions<SettingsAzureKeyVault> optionsKeyVault,
            ISystemClock clock, 
            IKeyMaterialService keys, 
            IdentityServerOptions options, 
            ILogger<DefaultTokenCreationService> logger,
            ILogger<AzureKeyVaultTokenCreationService> loggerHere,
            IWebHostEnvironment environment,
            IAzureKeyService azureKeyService)
            : base(clock, keys, options, logger)
        {
            _settings = optionsKeyVault.Value;
            _environment = environment;
            _logger = loggerHere;
            _azureKeyService = azureKeyService;

            _signingKeyName = _settings.SigningKeyName;
            _vaultUrl = $"https://{_settings.VaultName}.vault.azure.net/";
        }

        private async Task<CryptographyClient> GetClient()
        {
            var keys = await _azureKeyService.GetSigningKeysAsync();
            var currentVersion = keys.Current.Version;
            var keyUrl = $"{_vaultUrl}keys/{_signingKeyName}/{currentVersion}";
            _logger.LogInformation($"_keyUrl= {keyUrl}");

            var client = AzureClientsCreator.GetCryptographyClient(_settings, keyUrl, _environment.IsDevelopment());
            
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
