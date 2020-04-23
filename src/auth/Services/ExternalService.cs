using IdentityModel;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Test.model.Users;

namespace Test.auth.Services
{
    public interface IExternalService
    {
        Task<ApplicationUser> AutoProvisionUserAsync(string provider, string providerUserId, IEnumerable<Claim> claims);
    }
    public class ExternalService : IExternalService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IClaimsHelper _claimsHelper;

        public ExternalService(UserManager<ApplicationUser> userManager,
            IClaimsHelper claimsHelper)
        {
            _userManager = userManager;
            _claimsHelper = claimsHelper;
        }
        public async Task<ApplicationUser> AutoProvisionUserAsync(string provider, string providerUserId, IEnumerable<Claim> claims)
        {
            var filtered = new List<Claim>();

            var name = _claimsHelper.GetFullName(claims);
            if (!string.IsNullOrEmpty(name))
                filtered.Add(new Claim(JwtClaimTypes.Name, name));

            // email
            var email = _claimsHelper.GetEmail(claims);
            if (!string.IsNullOrEmpty(email))
                filtered.Add(new Claim(JwtClaimTypes.Email, email));

            var user = new ApplicationUser
            {
                UserName = Guid.NewGuid().ToString(),
                FullName = name,
                Email = email
            };
            var identityResult = await _userManager.CreateAsync(user);
            if (!identityResult.Succeeded) throw new Exception(identityResult.Errors.First().Description);

            if (filtered.Any())
            {
                identityResult = await _userManager.AddClaimsAsync(user, filtered);
                if (!identityResult.Succeeded) throw new Exception(identityResult.Errors.First().Description);
            }

            identityResult = await _userManager.AddLoginAsync(user, new UserLoginInfo(provider, providerUserId, provider));
            if (!identityResult.Succeeded) throw new Exception(identityResult.Errors.First().Description);

            return user;
        }
    }
}
