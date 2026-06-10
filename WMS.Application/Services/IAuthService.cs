using WMS.Application.DTOs.Auth;

namespace WMS.Application.Services
{
    /// <summary>
    /// Service interface for authentication operations.
    /// Handles login, logout, and user creation. Uses a single JWT token (no refresh tokens).
    /// </summary>
    public interface IAuthService
    {
        // Verify credentials and return a JWT token
        Task<LoginResponseDto?> LoginAsync(LoginRequestDto request);

        // Clear user session (no-op for stateless JWT, but kept for future use)
        Task<bool> LogoutAsync(int userId);

        // Admin action — revoke all tokens for any user
        Task<bool> RevokeAllTokensAsync(int userId);

        // Admin action — create login credentials for an existing employee
        Task<bool> CreateUserAsync(CreateUserDto request);
    }
}
