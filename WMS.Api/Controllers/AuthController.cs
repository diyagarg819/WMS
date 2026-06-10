using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WMS.Application.Common;
using WMS.Application.DTOs.Auth;
using WMS.Application.Services;

namespace WMS.Api.Controllers
{
    /// <summary>
    /// Handles authentication: login, token refresh, logout, token revocation, and user creation.
    /// Login and refresh endpoints are public. All others require authentication.
    /// There is NO register endpoint — this is an enterprise system where Admin creates logins.
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

        // POST /api/auth/login — public, returns access + refresh tokens
        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto request)
        {
            var loginResult = await _authService.LoginAsync(request);

            if (loginResult == null)
            {
                return Unauthorized(new ApiResponse<object>(false, "Invalid username or password"));
            }

            return Ok(new ApiResponse<LoginResponseDto>(true, "Login successful", loginResult));
        }

        // POST /api/auth/refresh — public, body: { refreshToken }, returns new tokens
        [AllowAnonymous]
        [HttpPost("refresh")]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequestDto request)
        {
            var refreshResult = await _authService.RefreshTokenAsync(request.RefreshToken);

            if (refreshResult == null)
            {
                return Unauthorized(new ApiResponse<object>(false, "Invalid or expired refresh token"));
            }

            return Ok(new ApiResponse<LoginResponseDto>(true, "Token refreshed successfully", refreshResult));
        }

        // POST /api/auth/logout — authenticated, clears refresh token for the current user
        [Authorize]
        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            // Read the user ID from the JWT claims — never trust the request body for identity
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userIdClaim == null || !int.TryParse(userIdClaim, out int userId))
            {
                return Unauthorized(new ApiResponse<object>(false, "Unable to identify user from token"));
            }

            var logoutSuccess = await _authService.LogoutAsync(userId);

            if (!logoutSuccess)
            {
                return NotFound(new ApiResponse<object>(false, "User not found"));
            }

            return Ok(new ApiResponse<object>(true, "Logged out successfully"));
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
                // Could be: employee not found, or username already taken
                return Conflict(new ApiResponse<object>(false, "Employee not found or username already taken"));
            }

            return StatusCode(201, new ApiResponse<object>(true, "User login created successfully"));
        }
    }
}
