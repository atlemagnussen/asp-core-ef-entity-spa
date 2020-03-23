using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Test.core.Services;
using Test.dataaccess.Data;
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
            _logger.LogInformation("hello");
            var users = await _authDbContext.Users.ToArrayAsync();
            foreach (var user in users)
            {
                //var claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity())
                //var manUser = _userManager.GetUserAsync()
                yield return new ListUserViewModel(user);
            }
        }

        [HttpPost]
        [Route("register")]
        public async Task<IActionResult> Register([FromBody]RegisterRequestViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var user = await _registerService.NewUser(model);
                return Ok(new RegisterResponseViewModel(user));
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
