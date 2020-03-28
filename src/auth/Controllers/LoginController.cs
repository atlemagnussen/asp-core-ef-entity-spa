using System;
using System.Threading.Tasks;
using IdentityServer4.Events;
using IdentityServer4.Extensions;
using IdentityServer4.Models;
using IdentityServer4.Services;
using IdentityServer4.Stores;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Test.auth.Attributes;
using Test.auth.Extentions;
using Test.auth.Models;
using Test.auth.Services;
using Test.model.Users;

namespace Test.auth.Controllers
{
    [SecurityHeaders]
    [AllowAnonymous]
    public class LoginController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IIdentityServerInteractionService _interaction;
        private readonly IClientStore _clientStore;
        private readonly IEventService _events;
        private readonly ILoginService _loginService;

        public LoginController(
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

        [HttpGet]
        public async Task<IActionResult> Index(string returnUrl)
        {
            var vm = await _loginService.BuildLoginViewModelAsync(returnUrl);

            if (vm.IsExternalLoginOnly)
            {
                return RedirectToAction("Challenge", "External", new { provider = vm.ExternalLoginScheme, returnUrl });
            }

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(LoginInputModel model, string button)
        {
            var context = await _interaction.GetAuthorizationContextAsync(model.ReturnUrl);

            // the user clicked the "cancel" button
            if (button != "login")
            {
                if (context != null)
                {
                    // if the user cancels, send a result back into IdentityServer as if they 
                    // denied the consent (even if this client does not require consent).
                    // this will send back an access denied OIDC error response to the client.
                    await _interaction.GrantConsentAsync(context, ConsentResponse.Denied);

                    // we can trust model.ReturnUrl since GetAuthorizationContextAsync returned non-null
                    if (await _clientStore.IsPkceClientAsync(context.ClientId))
                    {
                        // if the client is PKCE then we assume it's native, so this change in how to
                        // return the response is for better UX for the end user.
                        return this.LoadingPage("Redirect", model.ReturnUrl);
                    }

                    return Redirect(model.ReturnUrl);
                }
                else
                {
                    return Redirect("~/");
                }
            }

            if (ModelState.IsValid)
            {
                var result = await _signInManager.PasswordSignInAsync(model.Username, model.Password, model.RememberLogin, lockoutOnFailure: true);
                if (result.Succeeded)
                {
                    var user = await _userManager.FindByNameAsync(model.Username);
                    await _events.RaiseAsync(new UserLoginSuccessEvent(user.UserName, user.Id, user.UserName, clientId: context?.ClientId));

                    if (context != null)
                    {
                        if (await _clientStore.IsPkceClientAsync(context.ClientId))
                        {
                            // if the client is PKCE then we assume it's native, so this change in how to
                            // return the response is for better UX for the end user.
                            return this.LoadingPage("Redirect", model.ReturnUrl);
                        }

                        // we can trust model.ReturnUrl since GetAuthorizationContextAsync returned non-null
                        return Redirect(model.ReturnUrl);
                    }

                    // request for a local page
                    if (Url.IsLocalUrl(model.ReturnUrl))
                    {
                        return Redirect(model.ReturnUrl);
                    }
                    else if (string.IsNullOrEmpty(model.ReturnUrl))
                    {
                        return Redirect("~/");
                    }
                    else
                    {
                        // user might have clicked on a malicious link - should be logged
                        throw new Exception("invalid return URL");
                    }
                }

                await _events.RaiseAsync(new UserLoginFailureEvent(model.Username, "invalid credentials", clientId: context?.ClientId));
                ModelState.AddModelError(string.Empty, AccountOptions.InvalidCredentialsErrorMessage);
            }

            var vm = await _loginService.BuildLoginViewModelAsync(model);
            return View(vm);
        }

        [HttpGet]
        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}