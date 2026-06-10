using WMS.Application.DTOs.Auth;

namespace WMS.Application.Services
{
    /// <summary>
    /// Service interface for authentication operations.
    /// Handles login, token refresh, logout, token revocation, and user creation.
    /// </summary>
    public interface IAuthService
    {
        // Verify credentials and return access + refresh tokens
        Task<LoginResponseDto?> LoginAsync(LoginRequestDto request);

        // Validate refresh token, rotate it, and return new token pair
        Task<LoginResponseDto?> RefreshTokenAsync(string refreshToken);

        // Invalidate the refresh token for the current user
        Task<bool> LogoutAsync(int userId);

        // Admin action — revoke all tokens for any user
        Task<bool> RevokeAllTokensAsync(int userId);

        // Admin action — create login credentials for an existing employee
        Task<bool> CreateUserAsync(CreateUserDto request);
    }
}
