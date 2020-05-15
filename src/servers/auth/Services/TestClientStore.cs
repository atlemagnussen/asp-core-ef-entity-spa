using IdentityServer4.Models;
using IdentityServer4.Stores;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Test.auth.Services
{
    public class TestClientStore : IClientStore
    {
        private readonly IHardCodedClientsService _hardCodedClients;

        public TestClientStore(IHardCodedClientsService hardCodedClients)
        {
            _hardCodedClients = hardCodedClients;
        }

        public async Task<Client> FindClientByIdAsync(string clientId)
        {
            var hardCodedClient = await Task.FromResult(_hardCodedClients.Get(clientId));
            return hardCodedClient;
        }
    }
}
