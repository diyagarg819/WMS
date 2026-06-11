using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using WMS.Application.Common;
using WMS.Application.DTOs.Auth;
using WMS.Domain.Interfaces;

namespace WMS.Application.Services
{
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

        public async Task<LoginResponseDto?> LoginAsync(LoginRequestDto request)
        {
            var user = await _userLoginRepository.GetByUsernameAsync(request.Username);
            if (user == null)
            {
                _logger.LogWarning("Login failed — username not found: {Username}", request.Username);
                return null;
            }

            if (!BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            {
                _logger.LogWarning("Login failed — wrong password for username: {Username}", request.Username);
                return null;
            }

            string roleName = user.Role?.RoleName ?? "Employee";
            string token = GenerateJwtToken(user.UserId, user.Username, roleName);

            user.LastLogin = DateTime.UtcNow;
            await _userLoginRepository.UpdateAsync(user);

            _logger.LogInformation("Login successful for username: {Username}", request.Username);

            return new LoginResponseDto
            {
                AccessToken = token,
                ExpiresIn = _jwtSettings.AccessTokenExpiryMinutes * 60,
                Role = roleName,
                Username = user.Username
            };
        }

        private string GenerateJwtToken(int userId, string username, string roleName)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.SecretKey));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

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
