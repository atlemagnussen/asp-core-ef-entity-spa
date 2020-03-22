using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;
using System;
using System.Linq;
using Test.auth.Models;
using Test.model.Users;

namespace Test.auth.Services
{
    public interface IRegisterService
    {
        Task<ApplicationUser> NewUser(RegisterRequestViewModel model);
        Task EnsureRoles();
    }

    public class RegisterService : IRegisterService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public RegisterService(UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task EnsureRoles()
        {
            var roleAdminExists = await _roleManager.FindByNameAsync(SystemRoles.Admin);
            if (roleAdminExists == null)
            {
                var adminRole = new IdentityRole(SystemRoles.Admin);
                await _roleManager.CreateAsync(adminRole);
            }
        }
        public async Task<ApplicationUser> NewUser(RegisterRequestViewModel model)
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
                    await _userManager.AddClaimAsync(user, new System.Security.Claims.Claim("userName", user.UserName));
                    await _userManager.AddClaimAsync(user, new System.Security.Claims.Claim("email", user.Email));
                    await _userManager.AddClaimAsync(user, new System.Security.Claims.Claim("name", user.FullName));
                }
                if (userExists != null || result.Succeeded)
                {
                    await EnsureRoles();

                    var userHasRole = await _userManager.IsInRoleAsync(user, SystemRoles.Admin);
                    if (!userHasRole)
                        await _userManager.AddToRoleAsync(user, SystemRoles.Admin);

                    return user;
                }
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
            var resultStrings = result.Errors.Select(e => $"{e.Code.ToString()} - {e.Description}");
            throw new ApplicationException(string.Join(',', resultStrings));
        }
    }
}