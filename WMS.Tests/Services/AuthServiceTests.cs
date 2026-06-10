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
    /// <summary>
    /// Unit tests for AuthService — covers login, token refresh, logout, revocation, and user creation.
    /// Uses Moq to mock the repository layer so tests run without a database.
    /// </summary>
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

            // Use test-only JWT settings — these never appear in production
            var jwtSettings = new JwtSettings
            {
                SecretKey = "TestSecretKeyForUnitTestsThatIsAtLeast32Characters!",
                Issuer = "WMS-API-Test",
                Audience = "WMS-Client-Test",
                AccessTokenExpiryMinutes = 10,
                RefreshTokenExpiryMinutes = 60
            };
            _jwtOptions = Options.Create(jwtSettings);

            _authService = new AuthService(_mockRepository.Object, _jwtOptions, _mockLogger.Object);
        }

        // ── Login Tests ───────────────────────────────────────────────────

        [Fact]
        public async Task Login_WithCorrectCredentials_ReturnsTokens()
        {
            // Arrange — create a user with a known password hash
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

            // Act
            var result = await _authService.LoginAsync(request);

            // Assert — should return tokens with correct expiry
            Assert.NotNull(result);
            Assert.False(string.IsNullOrEmpty(result.AccessToken));
            Assert.False(string.IsNullOrEmpty(result.RefreshToken));
            Assert.Equal(600, result.ExpiresIn); // 10 minutes = 600 seconds
        }

        [Fact]
        public async Task Login_WithWrongPassword_ReturnsNull()
        {
            // Arrange — password hash does not match the provided password
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

            // Act
            var result = await _authService.LoginAsync(request);

            // Assert — login should fail and return null
            Assert.Null(result);
        }

        [Fact]
        public async Task Login_WithNonExistentUsername_ReturnsNull()
        {
            // Arrange — no user found for this username
            _mockRepository.Setup(r => r.GetByUsernameAsync("unknown")).ReturnsAsync((UserLogin?)null);

            var request = new LoginRequestDto { Username = "unknown", Password = "AnyPassword" };

            // Act
            var result = await _authService.LoginAsync(request);

            // Assert
            Assert.Null(result);
        }

        // ── Token Refresh Tests ───────────────────────────────────────────

        [Fact]
        public async Task RefreshToken_WithValidToken_ReturnsNewTokens()
        {
            // Arrange — user has a valid, non-expired refresh token
            string currentRefreshToken = "valid-refresh-token-123";

            var user = new UserLogin
            {
                UserId = 1,
                Username = "admin",
                PasswordHash = "hash",
                RoleId = 1,
                Role = new Role { RoleId = 1, RoleName = "Admin" },
                RefreshToken = currentRefreshToken,
                RefreshTokenExpiry = DateTime.UtcNow.AddMinutes(30) // Still valid for 30 more minutes
            };

            _mockRepository.Setup(r => r.GetByRefreshTokenAsync(currentRefreshToken)).ReturnsAsync(user);
            _mockRepository.Setup(r => r.UpdateAsync(It.IsAny<UserLogin>())).Returns(Task.CompletedTask);

            // Act
            var result = await _authService.RefreshTokenAsync(currentRefreshToken);

            // Assert — should return new tokens
            Assert.NotNull(result);
            Assert.False(string.IsNullOrEmpty(result.AccessToken));
            Assert.False(string.IsNullOrEmpty(result.RefreshToken));

            // The new refresh token should be different from the old one (rotation)
            Assert.NotEqual(currentRefreshToken, result.RefreshToken);
        }

        [Fact]
        public async Task RefreshToken_WithExpiredToken_ReturnsNull()
        {
            // Arrange — user's refresh token has already expired
            string expiredToken = "expired-refresh-token";

            var user = new UserLogin
            {
                UserId = 1,
                Username = "admin",
                PasswordHash = "hash",
                RoleId = 1,
                Role = new Role { RoleId = 1, RoleName = "Admin" },
                RefreshToken = expiredToken,
                RefreshTokenExpiry = DateTime.UtcNow.AddMinutes(-10) // Expired 10 minutes ago
            };

            _mockRepository.Setup(r => r.GetByRefreshTokenAsync(expiredToken)).ReturnsAsync(user);

            // Act
            var result = await _authService.RefreshTokenAsync(expiredToken);

            // Assert — expired token should be rejected
            Assert.Null(result);
        }

        [Fact]
        public async Task RefreshToken_WithNonExistentToken_ReturnsNull()
        {
            // Arrange — this token does not exist in the database (already rotated or fake)
            _mockRepository.Setup(r => r.GetByRefreshTokenAsync("fake-token")).ReturnsAsync((UserLogin?)null);

            // Act
            var result = await _authService.RefreshTokenAsync("fake-token");

            // Assert — unknown token should be rejected
            Assert.Null(result);
        }

        // ── Logout Tests ──────────────────────────────────────────────────

        [Fact]
        public async Task Logout_WithValidUserId_ReturnsTrue()
        {
            // Arrange
            var user = new UserLogin
            {
                UserId = 1,
                Username = "admin",
                PasswordHash = "hash",
                RoleId = 1,
                RefreshToken = "some-token",
                RefreshTokenExpiry = DateTime.UtcNow.AddMinutes(30)
            };

            _mockRepository.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(user);
            _mockRepository.Setup(r => r.UpdateAsync(It.IsAny<UserLogin>())).Returns(Task.CompletedTask);

            // Act
            var result = await _authService.LogoutAsync(1);

            // Assert — logout should succeed and clear the refresh token
            Assert.True(result);
            _mockRepository.Verify(r => r.UpdateAsync(It.Is<UserLogin>(u =>
                u.RefreshToken == null && u.RefreshTokenExpiry == null
            )), Times.Once);
        }

        [Fact]
        public async Task Logout_WithInvalidUserId_ReturnsFalse()
        {
            // Arrange — user does not exist
            _mockRepository.Setup(r => r.GetByIdAsync(999)).ReturnsAsync((UserLogin?)null);

            // Act
            var result = await _authService.LogoutAsync(999);

            // Assert
            Assert.False(result);
        }

        // ── Create User Tests ─────────────────────────────────────────────

        [Fact]
        public async Task CreateUser_WithValidData_ReturnsTrue()
        {
            // Arrange — employee exists and username is available
            _mockRepository.Setup(r => r.EmployeeExistsAsync(1)).ReturnsAsync(true);
            _mockRepository.Setup(r => r.UsernameExistsAsync("newuser")).ReturnsAsync(false);
            _mockRepository.Setup(r => r.AddAsync(It.IsAny<UserLogin>())).Returns(Task.CompletedTask);

            var request = new CreateUserDto
            {
                EmployeeId = 1,
                Username = "newuser",
                Password = "SecurePassword123",
                RoleId = 3
            };

            // Act
            var result = await _authService.CreateUserAsync(request);

            // Assert
            Assert.True(result);
            _mockRepository.Verify(r => r.AddAsync(It.IsAny<UserLogin>()), Times.Once);
        }

        [Fact]
        public async Task CreateUser_WithNonExistentEmployee_ReturnsFalse()
        {
            // Arrange — employee does not exist
            _mockRepository.Setup(r => r.EmployeeExistsAsync(999)).ReturnsAsync(false);

            var request = new CreateUserDto
            {
                EmployeeId = 999,
                Username = "newuser",
                Password = "SecurePassword123",
                RoleId = 3
            };

            // Act
            var result = await _authService.CreateUserAsync(request);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task CreateUser_WithDuplicateUsername_ReturnsFalse()
        {
            // Arrange — employee exists but username is already taken
            _mockRepository.Setup(r => r.EmployeeExistsAsync(1)).ReturnsAsync(true);
            _mockRepository.Setup(r => r.UsernameExistsAsync("existinguser")).ReturnsAsync(true);

            var request = new CreateUserDto
            {
                EmployeeId = 1,
                Username = "existinguser",
                Password = "SecurePassword123",
                RoleId = 3
            };

            // Act
            var result = await _authService.CreateUserAsync(request);

            // Assert
            Assert.False(result);
        }
    }
}
