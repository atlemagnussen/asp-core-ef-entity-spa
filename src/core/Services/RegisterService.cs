using Microsoft.AspNetCore.Identity;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Test.model.Users;

namespace Test.core.Services
{
    public interface IRegisterService
    {
        Task<ApplicationUser> NewUser(RegisterRequestViewModel model);
        Task<ApplicationUser> GiveAdminRole(string userId);
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
                    await _userManager.AddClaimAsync(user, new Claim("userName", user.UserName));
                    await _userManager.AddClaimAsync(user, new Claim("email", user.Email));
                    await _userManager.AddClaimAsync(user, new Claim("name", user.FullName));
                }
                return user;
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
        }
    }
}
