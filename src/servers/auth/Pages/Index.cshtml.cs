using IdentityModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Linq;
using System.Threading.Tasks;

namespace Test.auth.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly IConfiguration _configuration;

        public IndexModel(ILogger<IndexModel> logger,
            IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
        }

        [ViewData]
        public string WebClientUrl { get; set; }
        public string LoggedInUser { get; set; }
        public string Role { get; set; }
        

        public async Task<IActionResult> OnGetAsync()
        {
            _logger.LogInformation("Index start");
            WebClientUrl = _configuration.GetValue<string>("WebClientUrl");

            var allClaims = User.Claims.ToList();
            LoggedInUser = allClaims.FirstOrDefault(c => c.Type == JwtClaimTypes.PreferredUserName)?.Value;
            Role = allClaims.FirstOrDefault(c => c.Type == JwtClaimTypes.Role)?.Value;
            // var identities = User.Identities.ToList();
            
            return Page();
        }
    }
}
