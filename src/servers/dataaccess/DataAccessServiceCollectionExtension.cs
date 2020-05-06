using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using Test.dataaccess.Services;

namespace Test.dataaccess
{
    public static class DataAccessServiceCollectionExtension
    {
        public static void AddCommonDataProtection(this IServiceCollection services,
            IConfigurationSection configAzKv,
            bool isDevelopment)
        {
            var settings = configAzKv.Get<SettingsAzureKeyVault>();
            var keyVaultClient = AzureClientsCreator.GetKeyVaultClient(settings, isDevelopment);
            var vaultUrl = $"https://{settings.VaultName}.vault.azure.net";
            var keyUrl = $"{vaultUrl}/keys/{settings.DataProtectionKeyName}";
            services.AddDataProtection()
                .PersistKeysToDbContext<DataProtectionDbContext>()
                .ProtectKeysWithAzureKeyVault(keyVaultClient, keyUrl)
                .SetApplicationName("asp-core-ef-is4-spa")
                .SetDefaultKeyLifetime(TimeSpan.FromDays(7));
        }
        public static void AddCommonIdentitySettings(this IServiceCollection services)
        {
            // password options
            services.Configure<IdentityOptions>(options =>
            {
                options.Password.RequiredLength = 8;
                options.Password.RequireDigit = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireUppercase = true;
                options.Password.RequireNonAlphanumeric = false;
            });
        }
    }
}
