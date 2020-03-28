using System.Threading.Tasks;
using IdentityServer4.Events;
using IdentityServer4.Extensions;
using IdentityServer4.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Test.auth.Attributes;
using Test.auth.Models;
using Test.auth.Services;
using Test.model.Users;

namespace Test.auth.Controllers
{
    [SecurityHeaders]
    [AllowAnonymous]
    public class LogoutController : Controller
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IEventService _events;
        private readonly ILogoutService _logoutService;

        public LogoutController(
            SignInManager<ApplicationUser> signInManager,
            IEventService events,
            ILogoutService logoutService)
        {
            _signInManager = signInManager;
            _events = events;
            _logoutService = logoutService;
        }

        [HttpGet]
        public async Task<IActionResult> Index(string logoutId)
        {
            var vm = await _logoutService.BuildLogoutViewModelAsync(logoutId, User);

            if (vm.ShowLogoutPrompt == false)
            {
                return await Index(vm);
            }

            return View(vm);
        }

        /// <summary>
        /// Handle logout page postback
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(LogoutInputModel model)
        {
            // build a model so the logged out page knows what to display
            var vm = await _logoutService.BuildLoggedOutViewModelAsync(model.LogoutId, User, HttpContext);

            if (User?.Identity.IsAuthenticated == true)
            {
                // delete local authentication cookie
                await _signInManager.SignOutAsync();

                // raise the logout event
                await _events.RaiseAsync(new UserLogoutSuccessEvent(User.GetSubjectId(), User.GetDisplayName()));
            }

            // check if we need to trigger sign-out at an upstream identity provider
            if (vm.TriggerExternalSignout)
            {
                // build a return URL so the upstream provider will redirect back
                // to us after the user has logged out. this allows us to then
                // complete our single sign-out processing.
                string url = Url.Action("Logout", new { logoutId = vm.LogoutId });

                // this triggers a redirect to the external provider for sign-out
                return SignOut(new AuthenticationProperties { RedirectUri = url }, vm.ExternalAuthenticationScheme);
            }

            return View("LoggedOut", vm);
        }

        [HttpGet]
        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}