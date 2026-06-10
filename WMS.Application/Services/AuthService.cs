using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
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
    /// Handles authentication: login, logout, and user creation.
    /// Uses BCrypt for password hashing and a single JWT token for access.
    /// No refresh tokens — keeps things simple and beginner-friendly.
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

        // ── Login ─────────────────────────────────────────────────────────
        // Check username + password, then return a single JWT token.
        public async Task<LoginResponseDto?> LoginAsync(LoginRequestDto request)
        {
            // Step 1: Find the user by username
            var user = await _userLoginRepository.GetByUsernameAsync(request.Username);
            if (user == null)
            {
                _logger.LogWarning("Login failed — username not found: {Username}", request.Username);
                return null;
            }

            // Step 2: Verify the password against the stored hash
            bool isPasswordValid = BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash);
            if (!isPasswordValid)
            {
                _logger.LogWarning("Login failed — wrong password for username: {Username}", request.Username);
                return null;
            }

            // Step 3: Get the user's role name (defaults to "Employee" if not set)
            string roleName = user.Role?.RoleName ?? "Employee";

            // Step 4: Generate a JWT token
            string token = GenerateJwtToken(user.UserId, user.Username, roleName);

            // Step 5: Update LastLogin timestamp
            user.LastLogin = DateTime.UtcNow;
            await _userLoginRepository.UpdateAsync(user);

            _logger.LogInformation("Login successful for username: {Username}", request.Username);

            // Step 6: Return the token
            return new LoginResponseDto
            {
                AccessToken = token,
                ExpiresIn = _jwtSettings.AccessTokenExpiryMinutes * 60,
                Role = roleName,
                Username = user.Username
            };
        }

        // ── Logout ────────────────────────────────────────────────────────
        // With a single JWT, logout is handled client-side by deleting the token.
        // This method exists so the API contract stays clean.
        public async Task<bool> LogoutAsync(int userId)
        {
            var user = await _userLoginRepository.GetByIdAsync(userId);
            if (user == null)
                return false;

            _logger.LogInformation("User logged out: {UserId}", userId);
            return true;
        }

        // ── Revoke Tokens (Admin) ─────────────────────────────────────────
        // With stateless JWT there's nothing to revoke server-side,
        // but we keep this endpoint for admin tooling.
        public async Task<bool> RevokeAllTokensAsync(int userId)
        {
            var user = await _userLoginRepository.GetByIdAsync(userId);
            if (user == null)
                return false;

            _logger.LogInformation("Token revocation requested for user: {UserId}", userId);
            return true;
        }

        // ── Create User (Admin) ───────────────────────────────────────────
        // Admin creates login credentials for an existing employee.
        public async Task<bool> CreateUserAsync(CreateUserDto request)
        {
            // Make sure the employee exists
            bool employeeExists = await _userLoginRepository.EmployeeExistsAsync(request.EmployeeId);
            if (!employeeExists)
            {
                _logger.LogWarning("Create user failed — employee not found: {EmployeeId}", request.EmployeeId);
                return false;
            }

            // Make sure the username isn't already taken
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

        // ── Helper: Generate JWT Token ────────────────────────────────────
        // Creates a signed JWT with user claims (id, username, role).
        private string GenerateJwtToken(int userId, string username, string roleName)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.SecretKey));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            // These claims are embedded in the token and read by the API for authorization
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
    }
}
