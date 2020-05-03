using Microsoft.Azure.KeyVault;
using Microsoft.Azure.Services.AppAuthentication;

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
    }
}
