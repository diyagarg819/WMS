using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using WMS.Application.Common;
using WMS.Application.DTOs.Auth;
using WMS.Domain.Entities;
using WMS.Domain.Interfaces;

namespace WMS.Application.Services
{
    /// <summary>
    /// Handles all authentication logic: login, token refresh, logout, revocation, and user creation.
    /// Uses BCrypt for password hashing and JWT for access tokens.
    /// Refresh tokens are random strings stored in the database — not JWTs.
    /// </summary>
    public class AuthService : IAuthService
    {
        private readonly IUserLoginRepository _userLoginRepository;
        private readonly JwtSettings _jwtSettings;
        private readonly ILogger<AuthService> _logger;

        public AuthService(
            IUserLoginRepository userLoginRepository,
            IOptions<JwtSettings> jwtSettings,
            ILogger<AuthService> logger)
        {
            _userLoginRepository = userLoginRepository;
            _jwtSettings = jwtSettings.Value;
            _logger = logger;
        }

        // Verify the user's credentials and issue a new access + refresh token pair
        public async Task<LoginResponseDto?> LoginAsync(LoginRequestDto request)
        {
            // Look up the user by username
            var user = await _userLoginRepository.GetByUsernameAsync(request.Username);
            if (user == null)
            {
                _logger.LogWarning("Login failed — username not found: {Username}", request.Username);
                return null;
            }

            // Verify the password against the stored hash
            bool isPasswordValid = BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash);
            if (!isPasswordValid)
            {
                _logger.LogWarning("Login failed — wrong password for username: {Username}", request.Username);
                return null;
            }

            // Get the role name for the JWT claims
            string roleName = user.Role?.RoleName ?? "Employee";

            // Generate the access token (short-lived JWT) and refresh token (random string)
            string accessToken = GenerateAccessToken(user.UserId, user.Username, roleName);
            string refreshToken = GenerateRefreshToken();

            // Save the refresh token and update last login timestamp
            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiry = DateTime.UtcNow.AddMinutes(_jwtSettings.RefreshTokenExpiryMinutes);
            user.LastLogin = DateTime.UtcNow;
            await _userLoginRepository.UpdateAsync(user);

            _logger.LogInformation("Login successful for username: {Username}", request.Username);

            return new LoginResponseDto
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                ExpiresIn = _jwtSettings.AccessTokenExpiryMinutes * 60
            };
        }

        // Validate the refresh token, rotate it, and return a new token pair
        public async Task<LoginResponseDto?> RefreshTokenAsync(string refreshToken)
        {
            // Find the user who owns this refresh token
            var user = await _userLoginRepository.GetByRefreshTokenAsync(refreshToken);
            if (user == null)
            {
                _logger.LogWarning("Token refresh failed — refresh token not found in database");
                return null;
            }

            // Check if the refresh token has expired
            if (user.RefreshTokenExpiry == null || user.RefreshTokenExpiry < DateTime.UtcNow)
            {
                _logger.LogWarning("Token refresh failed — refresh token expired for user: {UserId}", user.UserId);
                return null;
            }

            // Get the role name for the new JWT
            string roleName = user.Role?.RoleName ?? "Employee";

            // Issue a new access token and rotate the refresh token
            string newAccessToken = GenerateAccessToken(user.UserId, user.Username, roleName);
            string newRefreshToken = GenerateRefreshToken();

            // Save the new refresh token — the old one is now invalid
            user.RefreshToken = newRefreshToken;
            user.RefreshTokenExpiry = DateTime.UtcNow.AddMinutes(_jwtSettings.RefreshTokenExpiryMinutes);
            await _userLoginRepository.UpdateAsync(user);

            _logger.LogInformation("Token refresh successful for user: {UserId}", user.UserId);

            return new LoginResponseDto
            {
                AccessToken = newAccessToken,
                RefreshToken = newRefreshToken,
                ExpiresIn = _jwtSettings.AccessTokenExpiryMinutes * 60
            };
        }

        // Clear the refresh token so it can no longer be used
        public async Task<bool> LogoutAsync(int userId)
        {
            var user = await _userLoginRepository.GetByIdAsync(userId);
            if (user == null)
                return false;

            // Nulling out the refresh token invalidates it server-side
            user.RefreshToken = null;
            user.RefreshTokenExpiry = null;
            await _userLoginRepository.UpdateAsync(user);

            _logger.LogInformation("User logged out: {UserId}", userId);
            return true;
        }

        // Admin action — revoke all tokens for a specific user
        public async Task<bool> RevokeAllTokensAsync(int userId)
        {
            var user = await _userLoginRepository.GetByIdAsync(userId);
            if (user == null)
                return false;

            user.RefreshToken = null;
            user.RefreshTokenExpiry = null;
            await _userLoginRepository.UpdateAsync(user);

            _logger.LogInformation("All tokens revoked for user: {UserId}", userId);
            return true;
        }

        // Admin action — create login credentials for an existing employee
        public async Task<bool> CreateUserAsync(CreateUserDto request)
        {
            // Make sure the employee actually exists before creating a login
            bool employeeExists = await _userLoginRepository.EmployeeExistsAsync(request.EmployeeId);
            if (!employeeExists)
            {
                _logger.LogWarning("Create user failed — employee not found: {EmployeeId}", request.EmployeeId);
                return false;
            }

            // Make sure the username is not already taken
            bool usernameExists = await _userLoginRepository.UsernameExistsAsync(request.Username);
            if (usernameExists)
            {
                _logger.LogWarning("Create user failed — username already taken: {Username}", request.Username);
                return false;
            }

            // Hash the password — never store plain text
            string passwordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);

            var userLogin = new UserLogin
            {
                Username = request.Username,
                PasswordHash = passwordHash,
                RoleId = request.RoleId
            };

            await _userLoginRepository.AddAsync(userLogin);

            _logger.LogInformation("User login created for employee: {EmployeeId}, username: {Username}",
                request.EmployeeId, request.Username);
            return true;
        }

        // Generate a short-lived JWT access token with user claims
        private string GenerateAccessToken(int userId, string username, string roleName)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.SecretKey));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            // Claims included in the token — these are read by the API for authorization
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
                new Claim(ClaimTypes.Name, username),
                new Claim(ClaimTypes.Role, roleName)
            };

            var token = new JwtSecurityToken(
                issuer: _jwtSettings.Issuer,
                audience: _jwtSettings.Audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(_jwtSettings.AccessTokenExpiryMinutes),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        // Generate a random refresh token — this is NOT a JWT, just a random string
        private string GenerateRefreshToken()
        {
            var randomBytes = new byte[64];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomBytes);
            return Convert.ToBase64String(randomBytes);
        }
    }
}
