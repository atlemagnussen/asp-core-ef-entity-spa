using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace Test.webapi.Controllers
{
    [Route("")]
    [ApiController]
    public class DefaultController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public DefaultController(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public IEnumerable<string> Get()
        {
            return new[]
            {
                "Hello",
                "test app",
                "DotnetCore",
                GetConnectionString("BankDatabase"),
                _configuration.GetValue<string>("AuthServerUrl")
            };
        }
        private string GetConnectionString(string name)
        {
            var connectionString = _configuration.GetConnectionString(name);
            string[] settings = connectionString.Split(';');
            string conString = string.Empty;
            if (settings.Length > 0)
            {
                foreach (var setting in settings)
                {
                    if (setting.ToLower().StartsWith("password"))
                        continue;
                    conString = $"{conString}{setting};";
                }
            }
            return conString;
        }
    }
}
