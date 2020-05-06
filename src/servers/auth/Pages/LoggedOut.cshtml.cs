using Microsoft.AspNetCore.Mvc.RazorPages;
using Test.auth.Models;

namespace Test.auth.Pages
{
    public class LoggedOutModel : PageModel
    {
        //public string PostLogoutRedirectUri { get; set; }
        //public string ClientName { get; set; }
        //public string SignOutIframeUrl { get; set; }

        //public bool AutomaticRedirectAfterSignOut { get; set; }

        //public string LogoutId { get; set; }
        //public bool TriggerExternalSignout => ExternalAuthenticationScheme != null && ExternalAuthenticationScheme != "AzureAD";
        //public string ExternalAuthenticationScheme { get; set; }

        public LoggedOutViewModel Vm { get; set; }
        public void OnGet(LoggedOutViewModel vm)
        {
            Vm = vm;
        }
    }
}