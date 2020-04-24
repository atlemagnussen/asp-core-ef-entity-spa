using Azure;
using Azure.Identity;
using Azure.Security.KeyVault.Keys;
using IdentityServer4;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Threading.Tasks;
using Test.auth.Models;

namespace Test.auth.Services
{
    public interface IAzureKeyService
    {
        //Task<RsaSigningKeyModel> GetRsaSigningKeyVaultClient();
        RsaSigningKeyModel GetRsaSigningKeyClient();
        Task<RsaSigningKeyModel> GetRsaSigningKeyClientAsync();
        EcSigningKeyModel GetEcSigningKeyClient();
        Task<EcSigningKeyModel> GetEcSigningKeyClientAsync();
    }
    public class AzureKeyService : IAzureKeyService
    {
        private readonly ILogger<AzureKeyService> _logger;
        private readonly string _vaultUrl;
        private readonly KeyClient _keyClient;
        //private readonly KeyVaultClient _keyVaultClient;

        private static string RsaKeyName = "rsa-2048-core-auth";
        private static string EcKeyName = "ec-2048-core-auth";

        public AzureKeyService(IConfiguration configuration,
            ILogger<AzureKeyService> logger)
        {
            _logger = logger;

            _vaultUrl = $"https://{configuration["KeyVaultName"]}.vault.azure.net/";
            var vaultUri = new Uri(_vaultUrl);
            var tokenCredential = new DefaultAzureCredential();
            //var tokenCredential = new ManagedIdentityCredential();
            _keyClient = new KeyClient(vaultUri, tokenCredential);

            //var azureServiceTokenProvider = new AzureServiceTokenProvider();
            //_keyVaultClient = new KeyVaultClient(
            //    new KeyVaultClient.AuthenticationCallback(
            //        azureServiceTokenProvider.KeyVaultTokenCallback));
        }

        public RsaSigningKeyModel GetRsaSigningKeyClient()
        {
            _logger.LogInformation("Start GetEcSigningKey Sync");

            try
            {
                var response = _keyClient.GetKey(RsaKeyName);
                if (response != null && response.Value != null)
                {
                    var model = GetFromResponse(response.Value);
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

        public async Task<RsaSigningKeyModel> GetRsaSigningKeyClientAsync()
        {
            //_logger.LogInformation("Start GetEcSigningKey Async");
            try
            {
                var response = await _keyClient.GetKeyAsync(RsaKeyName);
                if (response != null)
                {
                    var model = GetFromResponse(response.Value);
                    return model;
                }
                else
                {
                    //_logger.LogError("GetEcSigningKey keyFrom was null");
                }
            }
            catch (Exception ex)
            {
                //_logger.LogError("Error GetEcSigningKey", ex);
                throw;
            }
            return null;
        }

        private RsaSigningKeyModel GetFromResponse(KeyVaultKey keyVaultKey)
        {
            var model = new RsaSigningKeyModel();
            var rsa = keyVaultKey.Key.ToRSA();
            model.Raw = rsa.ToString();
            model.KeyId = keyVaultKey.Properties.Version;
            model.Key = new RsaSecurityKey(rsa) { KeyId = model.KeyId };
            model.Algorithm = IdentityServerConstants.RsaSigningAlgorithm.PS256;
            model.KeyType = keyVaultKey.KeyType.ToString();
            model.CurveName = keyVaultKey.Key.CurveName.ToString();
            model.SignatureAlgorithm = rsa.SignatureAlgorithm;
            return model;
        }

        public EcSigningKeyModel GetEcSigningKeyClient()
        {
            _logger.LogInformation("Start GetEcSigningKey Sync");

            try
            {
                var response = _keyClient.GetKey(EcKeyName);
                if (response != null && response.Value != null)
                {
                    var model = GetFromKeyVaultKey(response.Value);
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

        public async Task<EcSigningKeyModel> GetEcSigningKeyClientAsync()
        {
            //_logger.LogInformation("Start GetEcSigningKey Async");
            
            try
            {
                var response = await _keyClient.GetKeyAsync(EcKeyName);
                if (response != null && response.Value != null)
                {
                    var model = GetFromKeyVaultKey(response.Value);
                }
                else
                {
                    throw new ApplicationException("GetEcSigningKey keyFrom was null");
                }
            }
            catch (Exception ex)
            {
                throw;
                //_logger.LogError("Error GetEcSigningKey", ex);
            }
            return null;
        }

        private EcSigningKeyModel GetFromKeyVaultKey(KeyVaultKey keyVaultKey)
        {
            var model = new EcSigningKeyModel();
            var ec = keyVaultKey.Key.ToECDsa();
            model.Raw = ec.ToString();

            model.KeyId = keyVaultKey.Properties.Version;
            model.Key = new ECDsaSecurityKey(ec) { KeyId = model.KeyId };
            if (keyVaultKey.Key.CurveName != null)
            {
                _logger.LogInformation($"CurveName: {keyVaultKey.Key.CurveName}");
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
                throw new NotSupportedException();

            if (keyCurveName.Value == KeyCurveName.P256)
                    return IdentityServerConstants.ECDsaSigningAlgorithm.ES256;
            
            if (keyCurveName.Value == KeyCurveName.P384)
                return IdentityServerConstants.ECDsaSigningAlgorithm.ES384;

            if (keyCurveName.Value == KeyCurveName.P521)
                return IdentityServerConstants.ECDsaSigningAlgorithm.ES512;
         
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
