using IdentityServer4.AspNetIdentity;
using IdentityServer4.Models;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using System.Threading.Tasks;
using Test.model.Users;

namespace Test.auth.Services
{
    public class TestProfileService : ProfileService<ApplicationUser>
    {
        //private readonly UserManager<ApplicationUser> _userManager;
        //private readonly RoleManager<IdentityRole> _roleManager;

        public TestProfileService(UserManager<ApplicationUser> userManager,
            IUserClaimsPrincipalFactory<ApplicationUser> claimsFactory) : base(userManager, claimsFactory)
        {
        }
        public async override Task GetProfileDataAsync(ProfileDataRequestContext context)
        {
            await base.GetProfileDataAsync(context);
            var user = await UserManager.GetUserAsync(context.Subject);

            //var claims = new List<Claim>
            //{
            //    new Claim("email", user.Email),
            //    new Claim("name", user.FullName)
            //};

            var isAdmin = await UserManager.IsInRoleAsync(user, SystemRoles.Admin);

            if (isAdmin)
                context.IssuedClaims.Add(new Claim("role", SystemRoles.Admin));
                

            //.AddRange(claims);
        }

        public async override Task IsActiveAsync(IsActiveContext context)
        {
            await base.IsActiveAsync(context);
        }
    }
}
