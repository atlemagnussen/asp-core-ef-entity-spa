using IdentityServer4.Services;
using System;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;

namespace Test.auth.ServicesIdentity
{
    public class TestCorsPolicyService : ICorsPolicyService
    {
        private readonly IConfiguration _configuration;

        public TestCorsPolicyService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public Task<bool> IsOriginAllowedAsync(string origin)
        {
            return Task.FromResult(IsValidSubdomain(origin));
        }
        private bool IsValidSubdomain(string origin)
        {
            var uri = new Uri(origin);
            var webClientUrl = _configuration.GetValue<string>("WebClientUrl");

            if (origin.Equals(webClientUrl, StringComparison.OrdinalIgnoreCase))
                return true;

            if (uri.Host == "localhost" && (uri.Port == 8080 || uri.Port == 8000))
                return true;

            return false;
        }
    }
}
