using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using WMS.Application.Common;
using WMS.Application.DTOs.Auth;
using WMS.Application.Services;
using WMS.Domain.Entities;
using WMS.Domain.Interfaces;

namespace WMS.Tests.Services
{
    public class AuthServiceTests
    {
        private readonly Mock<IUserLoginRepository> _mockRepository;
        private readonly Mock<ILogger<AuthService>> _mockLogger;
        private readonly IOptions<JwtSettings> _jwtOptions;
        private readonly AuthService _authService;

        public AuthServiceTests()
        {
            _mockRepository = new Mock<IUserLoginRepository>();
            _mockLogger = new Mock<ILogger<AuthService>>();

            var jwtSettings = new JwtSettings
            {
                SecretKey = "TestSecretKeyForUnitTestsThatIsAtLeast32Characters!",
                Issuer = "WMS-API-Test",
                Audience = "WMS-Client-Test",
                AccessTokenExpiryMinutes = 720
            };
            _jwtOptions = Options.Create(jwtSettings);

            _authService = new AuthService(_mockRepository.Object, _jwtOptions, _mockLogger.Object);
        }

        [Fact]
        public async Task Login_WithCorrectCredentials_ReturnsToken()
        {
            string password = "TestPassword123";
            string passwordHash = BCrypt.Net.BCrypt.HashPassword(password);

            var user = new UserLogin
            {
                UserId = 1,
                Username = "admin",
                PasswordHash = passwordHash,
                RoleId = 1,
                Role = new Role { RoleId = 1, RoleName = "Admin" }
            };

            _mockRepository.Setup(r => r.GetByUsernameAsync("admin")).ReturnsAsync(user);
            _mockRepository.Setup(r => r.UpdateAsync(It.IsAny<UserLogin>())).Returns(Task.CompletedTask);

            var request = new LoginRequestDto { Username = "admin", Password = password };

            var result = await _authService.LoginAsync(request);

            Assert.NotNull(result);
            Assert.False(string.IsNullOrEmpty(result.AccessToken));
            Assert.Equal(720 * 60, result.ExpiresIn);
        }

        [Fact]
        public async Task Login_WithWrongPassword_ReturnsNull()
        {
            var user = new UserLogin
            {
                UserId = 1,
                Username = "admin",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("CorrectPassword"),
                RoleId = 1,
                Role = new Role { RoleId = 1, RoleName = "Admin" }
            };

            _mockRepository.Setup(r => r.GetByUsernameAsync("admin")).ReturnsAsync(user);

            var request = new LoginRequestDto { Username = "admin", Password = "WrongPassword" };

            var result = await _authService.LoginAsync(request);

            Assert.Null(result);
        }

        [Fact]
        public async Task Login_WithNonExistentUsername_ReturnsNull()
        {
            _mockRepository.Setup(r => r.GetByUsernameAsync("unknown")).ReturnsAsync((UserLogin?)null);

            var request = new LoginRequestDto { Username = "unknown", Password = "AnyPassword" };

            var result = await _authService.LoginAsync(request);

            Assert.Null(result);
        }
    }
}
