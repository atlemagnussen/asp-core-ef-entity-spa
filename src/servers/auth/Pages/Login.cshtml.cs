using System;
using System.Threading.Tasks;
using IdentityServer4.Events;
using IdentityServer4.Models;
using IdentityServer4.Services;
using IdentityServer4.Stores;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Test.auth.Extentions;
using Test.auth.Models;
using Test.auth.Services;
using Test.model.Users;

namespace Test.auth.Pages
{
    public class LoginModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IIdentityServerInteractionService _interaction;
        private readonly IClientStore _clientStore;
        private readonly IEventService _events;
        private readonly ILoginService _loginService;

        public LoginModel(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            IIdentityServerInteractionService interaction,
            IClientStore clientStore,
            IEventService events,
            ILoginService loginService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _interaction = interaction;
            _clientStore = clientStore;
            _events = events;
            _loginService = loginService;
        }

        [BindProperty]
        public LoginViewModel Vm { get; set; }

        public async Task<IActionResult> OnGetAsync(string returnUrl)
        {
            Vm = await _loginService.BuildLoginViewModelAsync(returnUrl);

            if (Vm.IsExternalLoginOnly)
            {
                return RedirectToAction("Challenge", "External", new { provider = Vm.ExternalLoginScheme, returnUrl });
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(string button)
        {
            var context = await _interaction.GetAuthorizationContextAsync(Vm.ReturnUrl);

            if (button != "login")
            {
                if (context != null)
                {
                    await _interaction.GrantConsentAsync(context, new ConsentResponse());

                    if (await _clientStore.IsPkceClientAsync(context.Client.ClientId))
                    {
                        return this.GoToRedirectPage(Vm.ReturnUrl);
                    }

                    return Redirect(Vm.ReturnUrl);
                }
                else
                {
                    return Redirect("~/");
                }
            }

            if (ModelState.IsValid)
            {
                var result = await _signInManager.PasswordSignInAsync(Vm.Username, Vm.Password, Vm.RememberLogin, lockoutOnFailure: true);
                if (result.Succeeded)
                {
                    var user = await _userManager.FindByNameAsync(Vm.Username);
                    await _events.RaiseAsync(new UserLoginSuccessEvent(user.UserName, user.Id, user.UserName, clientId: context?.Client.ClientId));

                    if (context != null)
                    {
                        if (await _clientStore.IsPkceClientAsync(context.Client.ClientId))
                        {
                            return this.GoToRedirectPage(Vm.ReturnUrl);
                        }

                        return Redirect(Vm.ReturnUrl);
                    }

                    if (Url.IsLocalUrl(Vm.ReturnUrl))
                    {
                        return Redirect(Vm.ReturnUrl);
                    }
                    else if (string.IsNullOrEmpty(Vm.ReturnUrl))
                    {
                        return Redirect("~/");
                    }
                    else
                    {
                        // user might have clicked on a malicious link - should be logged
                        throw new Exception("invalid return URL");
                    }
                }

                await _events.RaiseAsync(new UserLoginFailureEvent(Vm.Username, "invalid credentials", clientId: context?.Client.ClientId));
                ModelState.AddModelError(string.Empty, AccountOptions.InvalidCredentialsErrorMessage);
            }

            Vm = await _loginService.BuildLoginViewModelAsync(Vm);
            return Page();
        }
    }
}