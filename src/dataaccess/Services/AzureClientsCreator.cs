using Azure.Identity;
using Azure.Security.KeyVault.Keys;
using Azure.Security.KeyVault.Keys.Cryptography;
using Microsoft.Azure.KeyVault;
using Microsoft.Azure.Services.AppAuthentication;
using System;

namespace Test.dataaccess.Services
{
    public static class AzureClientsCreator
    {
        public static KeyVaultClient GetKeyVaultClient(SettingsAzureKeyVault settings, bool isDevelopment)
        {
            if (isDevelopment)
            {
                var connectionString = $"RunAs=App;AppId={settings.ClientId};TenantId={settings.TenantId};AppKey={settings.ClientSecret}";
                var azureServiceTokenProvider = new AzureServiceTokenProvider(connectionString);
                var client = new KeyVaultClient(
                    new KeyVaultClient.AuthenticationCallback(
                        azureServiceTokenProvider.KeyVaultTokenCallback));
                return client;
            }
            else
            {
                var azureServiceTokenProvider = new AzureServiceTokenProvider();
                var client = new KeyVaultClient(
                    new KeyVaultClient.AuthenticationCallback(
                        azureServiceTokenProvider.KeyVaultTokenCallback));
                return client;
            }
        }

        public static KeyClient GetKeyClient(SettingsAzureKeyVault settings, bool isDevelopment)
        {
            var vaultUrl = $"https://{settings.VaultName}.vault.azure.net/";
            var vaultUri = new Uri(vaultUrl);
            if (isDevelopment)
            {
                var clientCredential = new ClientSecretCredential(settings.TenantId, settings.ClientId, settings.ClientSecret);
                return new KeyClient(vaultUri, clientCredential);
            }
            else
            {
                var tokenCredential = new DefaultAzureCredential();
                return new KeyClient(vaultUri, tokenCredential);
            }
        }

        public static CryptographyClient GetCryptographyClient(SettingsAzureKeyVault settings, string keyUrl, bool isDevelopment)
        {
            if (isDevelopment)
            {
                var clientCredential = new ClientSecretCredential(settings.TenantId, settings.ClientId, settings.ClientSecret);
                return new CryptographyClient(new Uri(keyUrl), clientCredential);
            }
            else
                return new CryptographyClient(new Uri(keyUrl), new DefaultAzureCredential());
        }
    }
}
