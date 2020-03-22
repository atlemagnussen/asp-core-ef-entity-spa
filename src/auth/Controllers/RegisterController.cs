using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Test.core.Services;

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
    }
}