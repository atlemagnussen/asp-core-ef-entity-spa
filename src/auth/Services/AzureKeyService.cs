using Azure.Identity;
using Azure.Security.KeyVault.Keys;
using IdentityServer4;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Threading.Tasks;
using Test.auth.Models;

namespace Test.auth.Services
{
    public interface IAzureKeyService
    {
        RsaSigningKeys GetRsaSigningKeys();
        Task<RsaSigningKeys> GetRsaSigningKeysAsync();
        EcSigningKeys GetEcSigningKeys();
        Task<EcSigningKeys> GetEcSigningKeysAsync();

        //RsaSigningKeyModel GetRsaSigningKey(string name, string version = null);
        //Task<RsaSigningKeyModel> GetRsaSigningKeyAsync(string name, string version = null);
        //EcSigningKeyModel GetEcSigningKey(string name, string version = null);
        //Task<EcSigningKeyModel> GetEcSigningKeyAsync(string name, string version = null);
    }
    public class AzureKeyService : IAzureKeyService
    {
        private readonly IWebHostEnvironment _environment;
        private readonly ILogger<AzureKeyService> _logger;
        private readonly string _vaultUrl;
        private readonly KeyClient _keyClient;
        public static string RsaKeyName = "rsa-2048-core-auth";
        public static string EcKeyName = "ec-2048-core-auth";

        public AzureKeyService(IWebHostEnvironment environment,
            IConfiguration configuration,
            ILogger<AzureKeyService> logger)
        {
            _environment = environment;
            _logger = logger;

            _vaultUrl = $"https://{configuration["KeyVaultName"]}.vault.azure.net/";
            var vaultUri = new Uri(_vaultUrl);

            if (_environment.IsDevelopment())
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
                //var tokenCredential = new ManagedIdentityCredential();
                _keyClient = new KeyClient(vaultUri, tokenCredential);
            }

        }

        /*
         api
         */
        public RsaSigningKeys GetRsaSigningKeys()
        {
            var model = new RsaSigningKeys();
            model.Current = GetRsaSigningKey(RsaKeyName);

            var propertiesPages = _keyClient.GetPropertiesOfKeyVersions(RsaKeyName).AsPages();
            foreach (var page in propertiesPages)
            {
                foreach (var keyProperties in page.Values)
                {
                    if (model.Current.Version == keyProperties.Version)
                        continue;
                    else
                        model.Previous = GetRsaSigningKey(RsaKeyName, keyProperties.Version);
                }
            }
            return model;
        }
        public async Task<RsaSigningKeys> GetRsaSigningKeysAsync()
        {
            var model = new RsaSigningKeys();
            model.Current = await GetRsaSigningKeyAsync(RsaKeyName);

            var propertiesPages = _keyClient.GetPropertiesOfKeyVersionsAsync(RsaKeyName).AsPages();
            await foreach (var page in propertiesPages)
            {
                foreach(var keyProperties in page.Values)
                {
                    if (model.Current.Version == keyProperties.Version)
                        continue;
                    else
                        model.Previous = await GetRsaSigningKeyAsync(RsaKeyName, keyProperties.Version);
                }
            }
            return model;
        }

        public EcSigningKeys GetEcSigningKeys()
        {
            var model = new EcSigningKeys();
            model.Current = GetEcSigningKey(EcKeyName);

            var propertiesPages = _keyClient.GetPropertiesOfKeyVersions(EcKeyName).AsPages();
            foreach (var page in propertiesPages)
            {
                foreach (var keyProperties in page.Values)
                {
                    if (keyProperties.Enabled.HasValue && !keyProperties.Enabled.Value)
                        continue;
                    if (model.Current.Version == keyProperties.Version)
                        continue;
                    else
                        model.Previous = GetEcSigningKey(EcKeyName, keyProperties.Version);
                }
            }
            return model;
        }
        public async Task<EcSigningKeys> GetEcSigningKeysAsync()
        {
            var model = new EcSigningKeys();
            model.Current = await GetEcSigningKeyAsync(EcKeyName);

            var propertiesPages = _keyClient.GetPropertiesOfKeyVersionsAsync(EcKeyName).AsPages();
            await foreach (var page in propertiesPages)
            {
                foreach (var keyProperties in page.Values)
                {
                    if (keyProperties.Enabled.HasValue && !keyProperties.Enabled.Value)
                        continue;
                    if (model.Current.Version == keyProperties.Version)
                        continue;
                    else
                        model.Previous = await GetEcSigningKeyAsync(EcKeyName, keyProperties.Version);
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
            return new RsaSigningKeyModel(name, "Failed");
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
            return new RsaSigningKeyModel(name, "Failed");
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
            return new EcSigningKeyModel(name, "Failed");
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
            return new EcSigningKeyModel(name, "Failed");
        }

        /*
         private
             */
        private RsaSigningKeyModel GetRsaFromKeyVaultKey(KeyVaultKey keyVaultKey)
        {
            var model = new RsaSigningKeyModel(keyVaultKey.Name, keyVaultKey.Properties.Version);
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
            var model = new EcSigningKeyModel(keyVaultKey.Name, keyVaultKey.Properties.Version);
            var ec = keyVaultKey.Key.ToECDsa();

            model.Raw = keyVaultKey.Key.ToString();
            model.Key = new ECDsaSecurityKey(ec) { KeyId = model.Version };
            if (keyVaultKey.Key.CurveName != null)
            {
                model.Algorithm = GetEcAlgorithm(keyVaultKey.Key.CurveName);
            }

            model.KeyType = keyVaultKey.KeyType.ToString();
            model.CurveName = keyVaultKey.Key.CurveName.ToString();

            model.SignatureAlgorithm = ec.SignatureAlgorithm;
            return model;
        }
        private IdentityServerConstants.ECDsaSigningAlgorithm GetEcAlgorithm(KeyCurveName? keyCurveName)
        {
            if (!keyCurveName.HasValue)
            {
                _logger.LogError("Curve no value");
                throw new NotSupportedException();
            }

            if (keyCurveName.Value == KeyCurveName.P256)
                    return IdentityServerConstants.ECDsaSigningAlgorithm.ES256;
            
            if (keyCurveName.Value == KeyCurveName.P384)
                return IdentityServerConstants.ECDsaSigningAlgorithm.ES384;

            if (keyCurveName.Value == KeyCurveName.P521)
                return IdentityServerConstants.ECDsaSigningAlgorithm.ES512;

            _logger.LogError("Curve not supported");
            throw new NotSupportedException();
        }
    }
}
