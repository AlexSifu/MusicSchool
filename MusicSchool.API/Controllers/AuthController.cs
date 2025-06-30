using Microsoft.AspNetCore.Mvc;
using MusicSchool.Core.DTOs;
using MusicSchool.Core.Interfaces;

namespace MusicSchool.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            var result = await _authService.LoginAsync(request);
            if (!result.Success)
                return Unauthorized(new { message = result.Message });

            return Ok(new { message = result.Message, token = result.Token });
        }

        [HttpGet("ping")]
        public IActionResult Ping()
        {
            return Ok("API funcionando");
        }
    }
}
