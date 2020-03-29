using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using Test.auth.Models;

namespace Test.auth.Controllers
{
    public class HomeController : Controller
    {
        private readonly IWebHostEnvironment _environment;
        private readonly ILogger<HomeController> _logger;

        private readonly IConfiguration _configuration;

        public HomeController(IWebHostEnvironment environment,
        ILogger<HomeController> logger,
        IConfiguration configuration)
        {
            _environment = environment;
            _logger = logger;
            _configuration = configuration;
        }

        public IActionResult Index()
        {
            // if (_environment.IsDevelopment())
            // {
                // only show in development
            return View(GetViewModel());
            // }

            // _logger.LogInformation("Homepage is disabled in production. Returning 404.");
            // return NotFound();
        }

        private HomeViewModel GetViewModel() {
            var vm = new HomeViewModel {
                ConnectionString = GetConStr("AuthDb"),
                AllowedClientUrl = _configuration.GetValue<string>("AllowedClientUrl"),
                AzureAdClientId = _configuration.GetValue<string>("AzureAd:ClientId")
            };
            return vm;
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

        public IActionResult Privacy()
        {
            return View();
        }
    }
}
