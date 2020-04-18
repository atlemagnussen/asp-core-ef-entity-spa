using System.Threading.Tasks;
using IdentityServer4.Events;
using IdentityServer4.Extensions;
using IdentityServer4.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Test.auth.Models;
using Test.auth.Services;
using Test.model.Users;

namespace Test.auth.Pages
{
    public class LogoutModel : PageModel
    {
        private readonly ILogger<LogoutModel> _logger;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IEventService _events;
        private readonly ILogoutService _logoutService;

        public LogoutModel(ILogger<LogoutModel> logger,
        SignInManager<ApplicationUser> signInManager,
            IEventService events,
            ILogoutService logoutService)
        {
            _logger = logger;
            _signInManager = signInManager;
            _events = events;
            _logoutService = logoutService;
        }

        public LogoutViewModel Vm { get; set; }

        public async Task OnGetAsync(string logoutId)
        {
            _logger.LogInformation("Loutout get");
            Vm = await _logoutService.BuildLogoutViewModelAsync(logoutId, User);

            //if (ViewModel.ShowLogoutPrompt == false)
            //{
            // return await Index(vm);
            //}
        }

        public async Task<IActionResult> OnPostAsync(LogoutInputModel model)
        {
            var vm = await _logoutService.BuildLoggedOutViewModelAsync(model.LogoutId, User, HttpContext);

            if (User?.Identity.IsAuthenticated == true)
            {
                await _signInManager.SignOutAsync();

                await _events.RaiseAsync(new UserLogoutSuccessEvent(User.GetSubjectId(), User.GetDisplayName()));
            }

            if (vm.TriggerExternalSignout)
            {
                string url = Url.Action("Logout", new { logoutId = vm.LogoutId });

                return SignOut(new AuthenticationProperties { RedirectUri = url }, vm.ExternalAuthenticationScheme);
            }

            return RedirectToPage("LoggedOut", vm);
            //return View("LoggedOut", vm);
            
            //if (returnUrl != null)
            //{
            //    return LocalRedirect(returnUrl);
            //}
            //else
            //{
            //    return RedirectToPage()
            //}
        }
    }
}