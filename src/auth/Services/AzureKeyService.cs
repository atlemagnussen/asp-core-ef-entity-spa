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
            string raw = _configuration.GetValue<string>(RsaKeyName);
            var model = new RsaSigningKeyModel
            {
                Raw = raw
            };

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
            return model;
        }

        public EcSigningKeyModel GetEcSigningKey()
        {
            string raw = _configuration.GetValue<string>(EcKeyName);
            var model = new EcSigningKeyModel
            {
                Raw = raw
            };

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
            return model;
        }
    }
}
