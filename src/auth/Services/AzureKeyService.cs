using Azure.Identity;
using Azure.Security.KeyVault.Keys;
using IdentityServer4;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Test.auth.Models;

namespace Test.auth.Services
{
    public interface IAzureKeyService
    {
        Task<EcSigningKeys> GetSigningKeysAsync();
    }
    public class AzureKeyService : IAzureKeyService
    {
        private readonly ILogger<AzureKeyService> _logger;
        private readonly string _vaultUrl;
        private readonly KeyClient _keyClient;
        private readonly string _signingKeyName;
        private IMemoryCache _cache;

        public AzureKeyService(IWebHostEnvironment environment,
            IConfiguration configuration,
            ILogger<AzureKeyService> logger,
            IMemoryCache memoryCache)
        {
            _logger = logger;
            _cache = memoryCache;
            _vaultUrl = $"https://{configuration["KeyVaultName"]}.vault.azure.net/";
            var vaultUri = new Uri(_vaultUrl);

            _signingKeyName = configuration.GetValue<string>("SigningKeyName");

            if (environment.IsDevelopment())
            {
                var clientId = configuration.GetValue<string>("AzureKeyVault:clientId");
                var tenantId = configuration.GetValue<string>("AzureKeyVault:tenantId");
                var clientSecret = configuration.GetValue<string>("AzureKeyVault:clientSecret");
                var clientCredential = new ClientSecretCredential(tenantId, clientId, clientSecret);
                _keyClient = new KeyClient(vaultUri, clientCredential);
            }
            else
            {
                var tokenCredential = new DefaultAzureCredential();
                _keyClient = new KeyClient(vaultUri, tokenCredential);
            }
        }

        /*
         api
         */

        public async Task<EcSigningKeys> GetSigningKeysAsync()
        {
            var keys = await _cache.GetOrCreateAsync("CachedKeys", async entry =>
            {
                var expire = DateTimeOffset.Now.AddDays(1);
                entry.AbsoluteExpiration = expire;
                var keysAzure = await GetSigningKeysAzureAsync();
                keysAzure.CacheExpiring = expire;
                return keysAzure;
            });
            return keys;
        }

        protected async Task<EcSigningKeys> GetSigningKeysAzureAsync()
        {
            var model = new EcSigningKeys();
            var currentKeys = new List<EcSigningKeyModel>();
            var expiredKeys = new List<EcSigningKeyModel>();
            var futureKeys = new List<EcSigningKeyModel>();

            var propertiesPages = _keyClient.GetPropertiesOfKeyVersionsAsync(_signingKeyName).AsPages();
            await foreach (var page in propertiesPages)
            {
                foreach (var keyProperties in page.Values)
                {
                    if (keyProperties.Enabled.HasValue && !keyProperties.Enabled.Value)
                        continue;

                    var key = await GetEcSigningKeyAsync(_signingKeyName, keyProperties.Version);

                    if (key.Expired)
                    {
                        expiredKeys.Add(key);
                        continue;
                    }

                    if (key.Started)
                    {
                        currentKeys.Add(key);
                        continue;
                    }

                    futureKeys.Add(key);

                }
                if (futureKeys.Count > 0)
                {
                    if (futureKeys.Count == 1)
                        model.Future = futureKeys.First();
                }
                if (currentKeys.Count > 0)
                {
                    if (currentKeys.Count == 1)
                        model.Current = currentKeys.First();
                }
                if (expiredKeys.Count > 0)
                {
                    if (expiredKeys.Count == 1)
                        model.Previous = expiredKeys.First();
                }
            }
            return model;
        }

        /*
         */
        protected RsaSigningKeyModel GetRsaSigningKey(string name, string version = null)
        {
            try
            {
                var response = _keyClient.GetKey(name, version);
                if (response != null && response.Value != null)
                {
                    var model = GetRsaFromKeyVaultKey(response.Value);
                    return model;
                }
            }
            catch (Exception)
            {
            }
            return new RsaSigningKeyModel(name, "Failed", null, null);
        }

        protected async Task<RsaSigningKeyModel> GetRsaSigningKeyAsync(string name, string version = null)
        {
            _logger.LogInformation("Start GetEcSigningKey Async");
            try
            {
                var response = await _keyClient.GetKeyAsync(name, version);
                if (response != null)
                {
                    var model = GetRsaFromKeyVaultKey(response.Value);
                    return model;
                }
                else
                {
                    _logger.LogError("GetEcSigningKey keyFrom was null");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("Error GetEcSigningKey", ex);
            }
            return new RsaSigningKeyModel(name, "Failed", null, null);
        }

        protected EcSigningKeyModel GetEcSigningKey(string name, string version = null)
        {
            try
            {
                var response = _keyClient.GetKey(name, version);
                if (response != null && response.Value != null)
                {
                    var model = GetEcFromKeyVaultKey(response.Value);
                    return model;
                }
            }
            catch (Exception)
            {
            }
            return new EcSigningKeyModel(name, "Failed", null, null);
        }

        protected async Task<EcSigningKeyModel> GetEcSigningKeyAsync(string name, string version = null)
        {
            _logger.LogInformation("Start GetEcSigningKey Async");
            try
            {
                var response = await _keyClient.GetKeyAsync(name, version);
                if (response != null && response.Value != null)
                {
                    var model = GetEcFromKeyVaultKey(response.Value);
                    return model;
                }
                else
                {
                    _logger.LogError("GetEcSigningKey keyFrom was null");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("Error GetEcSigningKey", ex);
            }
            return new EcSigningKeyModel(name, "Failed", null, null);
        }

        /*
         private
             */
        private RsaSigningKeyModel GetRsaFromKeyVaultKey(KeyVaultKey keyVaultKey)
        {
            var model = new RsaSigningKeyModel(keyVaultKey.Name, keyVaultKey.Properties.Version, keyVaultKey.Properties.NotBefore, keyVaultKey.Properties.ExpiresOn);
            var rsa = keyVaultKey.Key.ToRSA();
            model.Raw = rsa.ToXmlString(false);
            model.Key = new RsaSecurityKey(rsa) { KeyId = model.Version };
            model.Algorithm = IdentityServerConstants.RsaSigningAlgorithm.PS256;
            model.KeyType = keyVaultKey.KeyType.ToString();
            model.CurveName = keyVaultKey.Key.CurveName.ToString();
            model.SignatureAlgorithm = rsa.SignatureAlgorithm;
            return model;
        }

        private EcSigningKeyModel GetEcFromKeyVaultKey(KeyVaultKey keyVaultKey)
        {
            var model = new EcSigningKeyModel(keyVaultKey.Name, keyVaultKey.Properties.Version, keyVaultKey.Properties.NotBefore, keyVaultKey.Properties.ExpiresOn);
            var ec = keyVaultKey.Key.ToECDsa();

            model.Raw = keyVaultKey.Key.ToString();
            model.Key = new ECDsaSecurityKey(ec) { KeyId = model.Version };
            if (keyVaultKey.Key.CurveName != null)
            {
                model.Algorithm = KeyCryptoHelper.GetEcAlgorithm(keyVaultKey.Key.CurveName);
            }

            model.KeyType = keyVaultKey.KeyType.ToString();
            model.CurveName = keyVaultKey.Key.CurveName.ToString();

            model.SignatureAlgorithm = ec.SignatureAlgorithm;
            return model;
        }
    }
}
