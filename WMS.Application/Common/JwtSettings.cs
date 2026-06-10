namespace WMS.Application.Common
{
    /// <summary>
    /// Strongly-typed settings for JWT token generation.
    /// Values are read from appsettings.json + user-secrets (for SecretKey).
    /// The SecretKey is never stored in appsettings.json — it comes from dotnet user-secrets.
    /// </summary>
    public class JwtSettings
    {
        public string SecretKey { get; set; } = string.Empty;
        public string Issuer { get; set; } = string.Empty;
        public string Audience { get; set; } = string.Empty;
        public int AccessTokenExpiryMinutes { get; set; } = 10;
        public int RefreshTokenExpiryMinutes { get; set; } = 60;
    }
}
