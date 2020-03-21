using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Test.auth.Models;
using Test.auth.Services;

namespace Test.auth.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RegisterController : ControllerBase
    {
        private readonly IRegisterService _registerService;
        public RegisterController(IRegisterService registerService)
        {
            _registerService = registerService;
        }

        [HttpGet]
        public async Task<IActionResult> EnsureRoles()
        {
            await _registerService.EnsureRoles();
            return Ok("ok");
        }

        [HttpPost]
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
            catch(ApplicationException ae)
            {
                return BadRequest(ae.Message);
            }
            catch(Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }
    }
}