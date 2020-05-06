using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Test.auth.Pages
{
    public class RedirectModel : PageModel
    {
        public string RedirectUrl { get; set; }
        public void OnGet(string redirectUrl)
        {
            RedirectUrl = redirectUrl;
        }
    }
}