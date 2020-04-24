using Azure.Identity;
using Azure.Security.KeyVault.Keys;
using IdentityServer4;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System;
using Test.auth.Models;

namespace Test.auth.Services
{
    public interface IAzureKeyService
    {
        public RsaSigningKeyModel GetRsaSigningKey();
        public EcSigningKeyModel GetEcSigningKey();
    }
    public class AzureKeyService : IAzureKeyService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<AzureKeyService> _logger;
        private readonly KeyClient _keyClient;

        private static string RsaKeyName = "rsa-2048-core-auth";
        private static string EcKeyName = "ec-2048-core-auth";

        public AzureKeyService(IConfiguration configuration,
            ILogger<AzureKeyService> logger)
        {
            _configuration = configuration;
            _logger = logger;

            var url = $"https://{configuration["KeyVaultName"]}.vault.azure.net/";
            var vaultUri = new Uri(url);
            //var tokenCredential = new DefaultAzureCredential();
            var tokenCredential = new ManagedIdentityCredential();
            _keyClient = new KeyClient(vaultUri, tokenCredential);
        }

        public RsaSigningKeyModel GetRsaSigningKey()
        {
            _logger.LogInformation("Start GetRsaSigningKey");
            var model = new RsaSigningKeyModel();
            try
            {
                string raw = _configuration[RsaKeyName];
                model.Raw = raw;

                _logger.LogInformation($"raw config key; {raw}");
                var keyFrom = _keyClient.GetKey(RsaKeyName);
                if (keyFrom != null)
                {
                    _logger.LogInformation($"{keyFrom.Value.KeyType}");
                    var rsa = keyFrom.Value.Key.ToRSA();
                    _logger.LogInformation($"Rsa: {rsa.ToString()}");
                    model.Key = new RsaSecurityKey(rsa);
                    model.Algorithm = IdentityServerConstants.RsaSigningAlgorithm.PS256;
                }
                else
                {
                    _logger.LogInformation("keyFrom was null");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("Error GetRsaSigningKey", ex);
            }
            return model;
        }

        public EcSigningKeyModel GetEcSigningKey()
        {
            _logger.LogInformation("Start GetEcSigningKey");
            var model = new EcSigningKeyModel();
            try
            {
                string raw = _configuration.GetValue<string>(EcKeyName);
                model.Raw = raw;

                _logger.LogInformation($"raw config key; {raw}");
                var keyFrom = _keyClient.GetKey(RsaKeyName);
                if (keyFrom != null)
                {
                    _logger.LogInformation($"{keyFrom.Value.KeyType}");
                    var ec = keyFrom.Value.Key.ToECDsa();
                    _logger.LogInformation($"Ec: {ec.ToString()}");
                    model.Key = new ECDsaSecurityKey(ec);
                    model.Algorithm = IdentityServerConstants.ECDsaSigningAlgorithm.ES256;
                }
                else
                {
                    _logger.LogInformation("keyFrom was null");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("Error GetEcSigningKey", ex);
            }
            return model;
        }
    }
}
