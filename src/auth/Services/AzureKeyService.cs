using Azure;
using Azure.Identity;
using Azure.Security.KeyVault.Keys;
using IdentityServer4;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Azure.KeyVault;
using Microsoft.Azure.KeyVault.Models;
using Microsoft.Azure.Services.AppAuthentication;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Net.Http;
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

        //Task<RsaSigningKeyModel> GetRsaSigningKeyVaultClient();
        RsaSigningKeyModel GetRsaSigningKey(string name, string version = null);
        Task<RsaSigningKeyModel> GetRsaSigningKeyAsync(string name, string version = null);
        EcSigningKeyModel GetEcSigningKey(string name, string version = null);
        Task<EcSigningKeyModel> GetEcSigningKeyAsync(string name, string version = null);
    }
    public class AzureKeyService : IAzureKeyService
    {
        private readonly IWebHostEnvironment _environment;
        private readonly ILogger<AzureKeyService> _logger;
        private readonly string _vaultUrl;
        private readonly KeyClient _keyClient;
        private readonly KeyVaultClient _keyVaultClient;

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

                var connectionString = $"RunAs=App;AppId={clientId};TenantId={tenantId};AppKey={clientSecret}";
                var azureServiceTokenProvider = new AzureServiceTokenProvider(connectionString);
                _keyVaultClient = new KeyVaultClient(
                    new KeyVaultClient.AuthenticationCallback(
                        azureServiceTokenProvider.KeyVaultTokenCallback));

            }
            else
            {
                var tokenCredential = new DefaultAzureCredential();
                //var tokenCredential = new ManagedIdentityCredential();
                _keyClient = new KeyClient(vaultUri, tokenCredential);
                var azureServiceTokenProvider = new AzureServiceTokenProvider();
                _keyVaultClient = new KeyVaultClient(
                    new KeyVaultClient.AuthenticationCallback(
                        azureServiceTokenProvider.KeyVaultTokenCallback));
            }

        }

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
        public RsaSigningKeyModel GetRsaSigningKey(string name, string version = null)
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
            return new RsaSigningKeyModel { Version = "Failed" };
        }

        public async Task<RsaSigningKeyModel> GetRsaSigningKeyAsync(string name, string version = null)
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
            return new RsaSigningKeyModel { Version = "Failed" };
        }

        public EcSigningKeyModel GetEcSigningKey(string name, string version = null)
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
            return new EcSigningKeyModel { Version = "Failed" };
        }

        public async Task<EcSigningKeyModel> GetEcSigningKeyAsync(string name, string version = null)
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
            return new EcSigningKeyModel { Version = "Failed" };
        }

        /*
         private
             */
        private RsaSigningKeyModel GetRsaFromKeyVaultKey(KeyVaultKey keyVaultKey)
        {
            var model = new RsaSigningKeyModel();
            var rsa = keyVaultKey.Key.ToRSA();
            model.Raw = rsa.ToXmlString(false);
            model.Version = keyVaultKey.Properties.Version;
            model.Key = new RsaSecurityKey(rsa) { KeyId = model.Version };
            model.Algorithm = IdentityServerConstants.RsaSigningAlgorithm.PS256;
            model.KeyType = keyVaultKey.KeyType.ToString();
            model.CurveName = keyVaultKey.Key.CurveName.ToString();
            model.SignatureAlgorithm = rsa.SignatureAlgorithm;
            return model;
        }

        private EcSigningKeyModel GetEcFromKeyVaultKey(KeyVaultKey keyVaultKey)
        {
            var model = new EcSigningKeyModel();
            var ec = keyVaultKey.Key.ToECDsa();

            model.Raw = keyVaultKey.Key.ToString();

            model.Version = keyVaultKey.Properties.Version;
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

        //public async Task<RsaSigningKeyModel> GetRsaSigningKeyVaultClient()
        //{
        //    _logger.LogInformation("Start GetRsaSigningKey");
        //    var model = new RsaSigningKeyModel();
        //    try
        //    {
        //        var keyBundle = await _keyVaultClient.GetKeyAsync(_vaultUrl, RsaKeyName);
        //        model.Raw = keyBundle.Key.ToString();

        //        if (keyBundle != null)
        //        {
        //            var rsa = keyBundle.Key.ToRSA();
        //            model.Key = new RsaSecurityKey(rsa);
        //            model.Algorithm = IdentityServerConstants.RsaSigningAlgorithm.PS256;
        //        }
        //        else
        //        {
        //            _logger.LogError("GetRsaSigningKey keyFrom was null");
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError("GetRsaSigningKey Error GetRsaSigningKey", ex.Message);
        //    }
        //    return model;
        //}

        //private void KeyVersions1()
        //{
        //    var versions = await _keyVaultClient.GetKeyVersionsAsync(_vaultUrl, RsaKeyName);
        //    while (versions != null)
        //    {
        //        versions = _keyVaultClient.GetKeysNextAsync(versions.NextPageLink).GetAwaiter().GetResult();

        //        foreach (var v in versions)

        //    }
        //}
    }
}
