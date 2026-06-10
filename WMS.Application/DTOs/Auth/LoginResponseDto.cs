namespace WMS.Application.DTOs.Auth
{
    /// <summary>
    /// Response returned after a successful login or token refresh.
    /// Contains the access token, refresh token, and when the access token expires.
    /// </summary>
    public class LoginResponseDto
    {
        public string AccessToken { get; set; } = string.Empty;

        public string RefreshToken { get; set; } = string.Empty;

        // Number of seconds until the access token expires (600 = 10 minutes)
        public int ExpiresIn { get; set; }
    }
}
