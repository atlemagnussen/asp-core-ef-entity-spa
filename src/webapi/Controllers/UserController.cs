using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Test.core.Services;
using Test.dataaccess;
using Test.model.Users;

namespace Test.webapi.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly ILogger<UserController> _logger;
        private readonly IRegisterService _registerService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly AuthDbContext _authDbContext;

        public UserController(ILogger<UserController> logger,
            IRegisterService registerService,
            UserManager<ApplicationUser> userManager,
            AuthDbContext authDbContext)
        {
            _logger = logger;
            _registerService = registerService;
            _authDbContext = authDbContext;
            _userManager = userManager;
        }

        [HttpGet]
        public async IAsyncEnumerable<ListUserViewModel> Get()
        {
            var code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes("hello"));
            _logger.LogInformation("hello");
            var users = await _authDbContext.Users.ToArrayAsync();
            foreach (var user in users)
            {
                var manUser = await _userManager.FindByEmailAsync(user.Email);
                var isAdmin = await _userManager.IsInRoleAsync(manUser, SystemRoles.Admin);
                yield return new ListUserViewModel(user, isAdmin);
            }
        }

        [HttpPost]
        [Route("register")]
        [Authorize("RequiresAdmin")]
        public async Task<IActionResult> Register([FromBody]RegisterRequestViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var result = await _registerService.NewUser(model, Request.Scheme);
                if (result.Succeeded)
                {
                    return Ok();
                }
                else
                {
                    return BadRequest(result.Errors);
                }
            }
            catch (ApplicationException ae)
            {
                return BadRequest(ae.Message);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }


        [HttpGet]
        [Route("ensureroles")]
        public async Task<IActionResult> EnsureRoles()
        {
            await _registerService.EnsureRoles();
            return Ok("ok");
        }

        [HttpGet]
        [Route("currentuserclaims")]
        public IActionResult CurrentUserClaims()
        {
            var claims = User.Claims.Select(claim => new { claim.Type, claim.Value }).ToArray();
            return Ok(claims);
        }
    }
}
