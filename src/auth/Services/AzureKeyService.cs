using Azure;
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
        //private readonly KeyVaultClient _keyVaultClient;

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
                var clientCredential = new ClientSecretCredential(
                tenantId: "38dd4d5d-cf60-48ff-9ebb-b16e94fa1c06",
                clientId: "3b147a3a-5818-440d-a22b-0a46a56f7b76",
                clientSecret: "73ceac53-8caa-49b1-9aae-de179be64715");
                _keyClient = new KeyClient(vaultUri, clientCredential);
            }
            else
            {
                var tokenCredential = new DefaultAzureCredential();
                //var tokenCredential = new ManagedIdentityCredential();
                _keyClient = new KeyClient(vaultUri, tokenCredential);
            }

            //var azureServiceTokenProvider = new AzureServiceTokenProvider();
            //_keyVaultClient = new KeyVaultClient(
            //    new KeyVaultClient.AuthenticationCallback(
            //        azureServiceTokenProvider.KeyVaultTokenCallback));
        }

        public RsaSigningKeys GetRsaSigningKeys()
        {
            return null;
        }
        public async Task<RsaSigningKeys> GetRsaSigningKeysAsync()
        {
            return null;
        }
        public EcSigningKeys GetEcSigningKeys()
        {
            return null;
        }
        public async Task<EcSigningKeys> GetEcSigningKeysAsync()
        {
            return null;
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
            return new RsaSigningKeyModel { KeyId = "Failed" };
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
            return new RsaSigningKeyModel { KeyId = "Failed" };
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
            return new EcSigningKeyModel { KeyId = "Failed" };
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
            return new EcSigningKeyModel { KeyId = "Failed" };
        }

        /*
         private
             */
        private RsaSigningKeyModel GetRsaFromKeyVaultKey(KeyVaultKey keyVaultKey)
        {
            var model = new RsaSigningKeyModel();
            var rsa = keyVaultKey.Key.ToRSA();
            model.Raw = rsa.ToXmlString(false);
            model.KeyId = keyVaultKey.Properties.Version;
            model.Key = new RsaSecurityKey(rsa) { KeyId = model.KeyId };
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

            model.KeyId = keyVaultKey.Properties.Version;
            model.Key = new ECDsaSecurityKey(ec) { KeyId = model.KeyId };
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
    }
}
