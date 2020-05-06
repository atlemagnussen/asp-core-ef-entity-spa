using IdentityModel;
using IdentityServer4.Stores;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Test.auth.Models;

namespace Test.auth.Services
{
    public interface IAdminService
    {
        Task<AdminViewModel> GetAdminViewModel();
        TopMenuViewModel GetTopMenuViewModel(ClaimsPrincipal user);
    }
    public class AdminService : IAdminService
    {
        private readonly IWebHostEnvironment _environment;
        private readonly ILogger<AdminService> _logger;
        private readonly IConfiguration _configuration;
        private readonly IAzureKeyService _azureKeyService;
        private readonly IClientStore _clientStore;
        private readonly IDataProtector _protector;

        public AdminService(IWebHostEnvironment environment,
            ILogger<AdminService> logger,
            IConfiguration configuration,
            IAzureKeyService azureKeyService,
            IClientStore clientStore,
            IDataProtectionProvider provider)
        {
            _environment = environment;
            _logger = logger;
            _configuration = configuration;
            _azureKeyService = azureKeyService;
            _clientStore = clientStore;
            _protector = provider.CreateProtector("test");
        }
        public async Task<AdminViewModel> GetAdminViewModel()
        {
            var vm = new AdminViewModel();
            try
            {
                vm.ProtectInput = "hello world 57666";
                vm.ProtectedInput = _protector.Protect(vm.ProtectInput);
                vm.UnprotectedInput = _protector.Unprotect(vm.ProtectedInput);
                vm.WebClient = await _clientStore.FindClientByIdAsync(Config.WebClientName);
                vm.AuthConnectionString = GetConStrStripPw("AuthDb");
                vm.BankConnectionString = GetConStrStripPw("BankDatabase");
                vm.AzureAdClientId = _configuration.GetValue<string>("AzureAd:ClientId");
                vm.IsDevelopment = _environment.IsDevelopment();
                vm.SigningKeys = await _azureKeyService.GetSigningKeysAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError("Terrible ERROR", ex);
            }
            return vm;
        }

        public TopMenuViewModel GetTopMenuViewModel(ClaimsPrincipal user)
        {
            var vm = new TopMenuViewModel();
            vm.WebClientUrl = _configuration.GetValue<string>("WebClientUrl");
            var allClaims = user.Claims.ToList();
            if (allClaims.Count > 0)
                vm.IsLoggedIn = false;
            var loggedInUser = allClaims.FirstOrDefault(c => c.Type == JwtClaimTypes.PreferredUserName)?.Value;
            if (loggedInUser != null)
                vm.IsLoggedIn = true;
            return vm;
        }

        private string GetConStrStripPw(string name) {
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
