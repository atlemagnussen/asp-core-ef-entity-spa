using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;

namespace Test.auth.Pages
{
    public class IndexModel : PageModel
    {
        private readonly IWebHostEnvironment _environment;
        private readonly ILogger<IndexModel> _logger;
        private readonly IConfiguration _configuration;

        public IndexModel(IWebHostEnvironment environment,
            ILogger<IndexModel> logger,
            IConfiguration configuration)
        {
            _environment = environment;
            _logger = logger;
            _configuration = configuration;
        }
        public string ConnectionString { get; set; }
        public string AllowedClientUrl { get; set; }
        public string AzureAdClientId { get; set; }
        public string ClientUrl { get; set; }

        public string Secret1 { get; set; }
        public string Secret2 { get; set; }
        public string Secret3 { get; set; }

        public void OnGet()
        {
            ConnectionString = GetConStr("AuthDb");
            AllowedClientUrl = _configuration.GetValue<string>("AllowedClientUrl");
            AzureAdClientId = _configuration.GetValue<string>("AzureAd:ClientId");
            ClientUrl = _configuration.GetValue<string>("AllowedClientUrl");
            Secret1 = _configuration["SecretName"];
            Secret2 = _configuration["Section:SecretName"];
            Secret3 = _configuration.GetSection("Section")["SecretName"];
    }

        private string GetConStr(string name) {
            var connectionString = _configuration.GetConnectionString(name);
            string[] settings = connectionString.Split(';');
            string conString = string.Empty;
            if (settings.Length > 0) {
                foreach (var setting in settings) {
                    if (setting.ToLower().StartsWith("password"))
                        continue;
                    conString = $"{conString}{setting};";
                }
            }
            return conString;
        }
    }
}
