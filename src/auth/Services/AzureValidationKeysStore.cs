using IdentityServer4.Models;
using IdentityServer4.Stores;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Test.auth.Services
{
    public class AzureValidationKeysStore : IValidationKeysStore
    {
        private ILogger<AzureValidationKeysStore> _logger;
        private readonly IAzureKeyService _azureKeyService;

        public AzureValidationKeysStore(ILogger<AzureValidationKeysStore> logger,
            IAzureKeyService azureKeyService)
        {
            _azureKeyService = azureKeyService;
            _logger = logger;
        }
        public async Task<IEnumerable<SecurityKeyInfo>> GetValidationKeysAsync()
        {
            _logger.LogInformation("AzureValidationKeysStore");
            var keys = await _azureKeyService.GetSigningKeysAsync();
            var list = new List<SecurityKeyInfo>();
            list.Add(keys.Current.GetSecurityKeyInfo());
            if (keys.Previous != null)
                list.Add(keys.Previous.GetSecurityKeyInfo());
            if (keys.Future != null)
                list.Add(keys.Future.GetSecurityKeyInfo());

            return list;
        }
    }
}
