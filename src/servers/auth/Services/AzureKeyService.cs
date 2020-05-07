using Azure.Security.KeyVault.Keys;
using IdentityServer4;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Test.auth.Models;
using Test.dataaccess;
using Test.dataaccess.Services;

namespace Test.auth.Services
{
    /// <summary>
    /// interface for fetching signing keys
    /// </summary>
    public interface IAzureKeyService
    {
        Task<SigningKeys> GetSigningKeysAsync();
    }
    public class AzureKeyService : IAzureKeyService
    {
        private readonly ILogger<AzureKeyService> _logger;
        private readonly KeyClient _keyClient;
        private readonly string _signingKeyName;
        private IMemoryCache _cache;

        public AzureKeyService(IOptions<SettingsAzureKeyVault> options,
            IWebHostEnvironment environment,
            ILogger<AzureKeyService> logger,
            IMemoryCache memoryCache)
        {
            _logger = logger;
            _cache = memoryCache;

            _signingKeyName = options.Value.SigningKeyName;

            _keyClient = AzureClientsCreator.GetKeyClient(options.Value, environment.IsDevelopment());
        }

        /*
         api
         */

        public async Task<SigningKeys> GetSigningKeysAsync()
        {
            var keys = await _cache.GetOrCreateAsync(_signingKeyName, async entry =>
            {
                var expire = DateTimeOffset.Now.AddHours(1);
                entry.AbsoluteExpiration = expire;
                var keysAzure = await GetSigningKeysAzureAsync();
                keysAzure.CacheExpiring = expire;
                return keysAzure;
            });
            _logger.LogDebug($"Got signingkeys '{_signingKeyName}', cached until {keys.CacheExpiring}");
            return keys;
        }

        /*
         internal
         */
        protected async Task<SigningKeys> GetSigningKeysAzureAsync()
        {
            var model = new SigningKeys();
            var currentKeys = new List<SigningKeyModel>();
            var expiredKeys = new List<SigningKeyModel>();
            var futureKeys = new List<SigningKeyModel>();

            var propertiesPages = _keyClient.GetPropertiesOfKeyVersionsAsync(_signingKeyName).AsPages();
            await foreach (var page in propertiesPages)
            {
                foreach (var keyProperties in page.Values)
                {
                    if (keyProperties.Enabled.HasValue && !keyProperties.Enabled.Value)
                        continue;

                    var key = await GetSigningKeyAsync(_signingKeyName, keyProperties.Version);

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
                    else
                        model.Future = GetClosestFutureKey(futureKeys);
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
                    else
                        model.Previous = GetClosestExpiredKey(expiredKeys);
                }
            }
            return model;
        }


        protected async Task<SigningKeyModel> GetSigningKeyAsync(string name, string version = null)
        {
            _logger.LogInformation("Start GetSigningKey Async");
            
            var response = await _keyClient.GetKeyAsync(name, version);
            if (response != null && response.Value != null)
            {
                var keyVaultKey = response.Value;
                if (keyVaultKey.KeyType == KeyType.Ec)
                    return GetEcFromKeyVaultKey(keyVaultKey);

                else if (keyVaultKey.KeyType == KeyType.Rsa)
                    return GetRsaFromKeyVaultKey(keyVaultKey);

                else
                    throw new NotSupportedException();
            }
            else
                throw new Exception("GetSigningKey keyFrom was null");
        }
        /*
         */

        /*
         private
        */

        private SigningKeyModel GetClosestExpiredKey(List<SigningKeyModel> expiredKeys)
        {
            var expiredKeysOrdered = expiredKeys.OrderByDescending(k => k.ExpiresOn);
            return expiredKeysOrdered.First();
        }
        private SigningKeyModel GetClosestFutureKey(List<SigningKeyModel> expiredKeys)
        {
            var expiredKeysOrdered = expiredKeys.OrderBy(k => k.NotBefore);
            return expiredKeysOrdered.First();
        }

        private SigningKeyModel GetRsaFromKeyVaultKey(KeyVaultKey keyVaultKey)
        {
            var model = new SigningKeyModel(keyVaultKey.Name, 
                keyVaultKey.Properties.Version, 
                keyVaultKey.Properties.NotBefore, 
                keyVaultKey.Properties.ExpiresOn);

            var rsa = keyVaultKey.Key.ToRSA();
            model.Key = new RsaSecurityKey(rsa) { KeyId = model.Version };
            model.AlgorithmString = IdentityServerConstants.RsaSigningAlgorithm.PS512.ToString();
            model.KeyType = keyVaultKey.KeyType.ToString();
            model.CurveName = keyVaultKey.Key.CurveName.ToString();
            model.SignatureAlgorithm = rsa.SignatureAlgorithm;
            return model;
        }

        private SigningKeyModel GetEcFromKeyVaultKey(KeyVaultKey keyVaultKey)
        {
            var model = new SigningKeyModel(keyVaultKey.Name, 
                keyVaultKey.Properties.Version, 
                keyVaultKey.Properties.NotBefore, 
                keyVaultKey.Properties.ExpiresOn);

            var ec = keyVaultKey.Key.ToECDsa();

            model.Key = new ECDsaSecurityKey(ec) { KeyId = model.Version };
            if (keyVaultKey.Key.CurveName != null)
            {
                var algorithm = KeyCryptoHelper.GetEcAlgorithm(keyVaultKey.Key.CurveName);
                model.AlgorithmString = KeyCryptoHelper.GetECDsaSigningAlgorithmValue(algorithm);
            }

            model.KeyType = keyVaultKey.KeyType.ToString();
            model.CurveName = keyVaultKey.Key.CurveName.ToString();

            model.SignatureAlgorithm = ec.SignatureAlgorithm;
            return model;
        }
    }
}
