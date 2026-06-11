using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WMS.Application.Common;
using WMS.Application.DTOs.Auth;
using WMS.Application.Services;

namespace WMS.Api.Controllers
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

        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto request)
        {
            var loginResult = await _authService.LoginAsync(request);

            if (loginResult == null)
                return Unauthorized(new ApiResponse<object>(false, "Invalid username or password"));

            return Ok(new ApiResponse<LoginResponseDto>(true, "Login successful", loginResult));
        }
    }
}
