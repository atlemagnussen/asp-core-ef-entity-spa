using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Security.Claims;
using System.Security.Policy;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Test.model.Users;

namespace Test.core.Services
{
    public interface IRegisterService
    {
        Task<IdentityResult> NewUser(RegisterRequestViewModel model, string scheme);
        Task<ApplicationUser> GiveAdminRole(string userId);
        Task EnsureRoles();
    }

    public class RegisterService : IRegisterService
    {
        private readonly ILogger<RegisterService> _logger;
        private readonly IConfiguration _configuration;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public RegisterService(ILogger<RegisterService> logger,
            IConfiguration configuration,
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager)
        {
            _logger = logger;
            _configuration = configuration;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task EnsureRoles()
        {
            var roleAdmin = await _roleManager.FindByNameAsync(SystemRoles.Admin);
            if (roleAdmin == null)
            {
                roleAdmin = new IdentityRole(SystemRoles.Admin);
                await _roleManager.CreateAsync(roleAdmin);
            }
        }

        public async Task<ApplicationUser> GiveAdminRole(string userId)
        {
            var user = await _userManager.FindByEmailAsync(userId);
            if (user != null)
            {
                await EnsureRoles();

                var userHasRole = await _userManager.IsInRoleAsync(user, SystemRoles.Admin);
                if (!userHasRole)
                    await _userManager.AddToRoleAsync(user, SystemRoles.Admin);
            }
            return user;
        }

        public async Task<IdentityResult> NewUser(RegisterRequestViewModel model, string scheme)
        {
            var user = new ApplicationUser
            {
                UserName = model.Email,
                Email = model.Email,
                FullName = model.Name
            };

            IdentityResult result = new IdentityResult();
            var userExists = await _userManager.FindByEmailAsync(model.Email);
            if (userExists == null)
                result = await _userManager.CreateAsync(user, model.Password);

            try
            {
                if (result.Succeeded)
                {
                    //await _userManager.AddClaimAsync(user, new Claim("userName", user.UserName));
                    //await _userManager.AddClaimAsync(user, new Claim("email", user.Email));
                    //await _userManager.AddClaimAsync(user, new Claim("name", user.FullName));
                    // await CreateConfirmationEmail(user, scheme);
                }
                return result;
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
        }

        public async Task<string> CreateConfirmationEmail(ApplicationUser user, string scheme)
        {
            _logger.LogInformation("User created a new account with password.");

            var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);

            code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));

            var callbackUrl = genUrl(user.Id, code);
            return HtmlEncoder.Default.Encode(callbackUrl);
            //await _emailSender.SendEmailAsync(user.Email, "Confirm your email",
            //    $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");

            //if (_userManager.Options.SignIn.RequireConfirmedAccount)
            //{
            //    return RedirectToPage("RegisterConfirmation",
            //                          new { email = Input.Email });
            //}
            //else
            //{
            //    await _signInManager.SignInAsync(user, isPersistent: false);
            //    return LocalRedirect(returnUrl);
            //}
        }

        public async Task ConfirmEmail(string userId, string code)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                throw new ApplicationException($"No user by id {userId}");

            await _userManager.ConfirmEmailAsync(user, code);
        }

        public string genUrl(string userId, string code)
        {
            var baseUrl = _configuration.GetValue<string>("AuthServerUrl");
            var url = $"{baseUrl}/ConfirmEmail?userId={userId}&code={code}";
            return url;
        }
    }
}
