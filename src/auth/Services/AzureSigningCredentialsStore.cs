using IdentityServer4.Stores;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System.Threading.Tasks;

namespace Test.auth.Services
{
    public class AzureSigningCredentialsStore : ISigningCredentialStore
    {
        private ILogger<AzureSigningCredentialsStore> _logger;
        private readonly IAzureKeyService _azureKeyService;

        public AzureSigningCredentialsStore(ILogger<AzureSigningCredentialsStore> logger,
            IAzureKeyService azureKeyService)
        {
            _logger = logger;
            _azureKeyService = azureKeyService;
        }
        public async Task<SigningCredentials> GetSigningCredentialsAsync()
        {
            _logger.LogInformation("AzureSigningCredentialsStore");
            var keyModel = await _azureKeyService.GetEcSigningKeyClientAsync();
            return new SigningCredentials(keyModel.Key, keyModel.Algorithm.ToString());
        }
    }
}
