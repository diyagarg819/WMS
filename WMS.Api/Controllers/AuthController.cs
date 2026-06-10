using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using WMS.Application.Common;
using WMS.Application.DTOs.Auth;
using WMS.Application.Services;

namespace WMS.Api.Controllers
{
    /// <summary>
    /// Handles authentication: login, logout, and user creation.
    /// Login is public and rate-limited. All other endpoints require authentication.
    /// Uses HttpOnly cookies for JWT to prevent XSS attacks.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly ILogger<AuthController> _logger;

        public AuthController(IAuthService authService, ILogger<AuthController> logger)
        {
            _authService = authService;
            _logger = logger;
        }

        // POST /api/auth/login — public, returns a JWT token in an HttpOnly cookie
        [AllowAnonymous]
        [HttpPost("login")]
        [EnableRateLimiting("LoginPolicy")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto request)
        {
            var loginResult = await _authService.LoginAsync(request);

            if (loginResult == null)
            {
                return Unauthorized(new ApiResponse<object>(false, "Invalid username or password"));
            }

            // Set the JWT as an HttpOnly, Secure cookie
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Secure = true, // Ensure we are on HTTPS or localhost
                SameSite = SameSiteMode.Strict,
                Expires = DateTime.UtcNow.AddSeconds(loginResult.ExpiresIn)
            };

            Response.Cookies.Append("jwt", loginResult.AccessToken, cookieOptions);

            return Ok(new ApiResponse<LoginResponseDto>(true, "Login successful", loginResult));
        }

        // POST /api/auth/logout — authenticated, logs out the current user
        [Authorize]
        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            // Clear the cookie
            Response.Cookies.Delete("jwt");

            // Read the user ID from the JWT claims
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userIdClaim == null || !int.TryParse(userIdClaim, out int userId))
            {
                // Even if we can't find the user ID, we already cleared the cookie
                return Ok(new ApiResponse<object>(true, "Logged out successfully"));
            }

            await _authService.LogoutAsync(userId);

            return Ok(new ApiResponse<object>(true, "Logged out successfully"));
        }

        // GET /api/auth/me — authenticated, returns current user info based on cookie
        [Authorize]
        [HttpGet("me")]
        public IActionResult GetCurrentUser()
        {
            var role = User.FindFirst(ClaimTypes.Role)?.Value ?? "Employee";
            var username = User.FindFirst(ClaimTypes.Name)?.Value ?? "Unknown";

            return Ok(new ApiResponse<object>(true, "User is authenticated", new { Role = role, Username = username }));
        }

        // POST /api/auth/revoke/{id} — Admin only, revoke any user's tokens
        [Authorize(Roles = "Admin")]
        [HttpPost("revoke/{id}")]
        public async Task<IActionResult> RevokeToken(int id)
        {
            var revokeSuccess = await _authService.RevokeAllTokensAsync(id);

            if (!revokeSuccess)
            {
                return NotFound(new ApiResponse<object>(false, "User not found"));
            }

            return Ok(new ApiResponse<object>(true, "All tokens revoked for user"));
        }

        // POST /api/auth/create-user — Admin only, create login credentials for an existing employee
        [Authorize(Roles = "Admin")]
        [HttpPost("create-user")]
        public async Task<IActionResult> CreateUser([FromBody] CreateUserDto request)
        {
            var createSuccess = await _authService.CreateUserAsync(request);

            if (!createSuccess)
            {
                return Conflict(new ApiResponse<object>(false, "Employee not found or username already taken"));
            }

            return StatusCode(201, new ApiResponse<object>(true, "User login created successfully"));
        }
    }
}
