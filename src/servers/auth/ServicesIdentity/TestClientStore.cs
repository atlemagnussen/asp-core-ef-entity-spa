using IdentityServer4.Models;
using IdentityServer4.Stores;
using System.Threading.Tasks;
using Test.auth.Services;

namespace Test.auth.ServicesIdentity
{
    public class TestClientStore : IClientStore
    {
        private readonly ITestClientsService _hardCodedClients;

        public TestClientStore(ITestClientsService hardCodedClients)
        {
            _hardCodedClients = hardCodedClients;
        }

        public async Task<Client> FindClientByIdAsync(string clientId)
        {
            var hardCodedClient = await _hardCodedClients.Get(clientId);
            return hardCodedClient;
        }
    }
}
