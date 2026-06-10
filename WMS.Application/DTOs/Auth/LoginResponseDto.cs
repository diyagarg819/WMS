using System.Text.Json.Serialization;

namespace WMS.Application.DTOs.Auth
{
    /// <summary>
    /// Response returned after a successful login.
    /// AccessToken is ignored in JSON because it is sent via an HttpOnly cookie.
    /// </summary>
    public class LoginResponseDto
    {
        [JsonIgnore]
        public string AccessToken { get; set; } = string.Empty;

        // Number of seconds until the access token expires (600 = 10 minutes)
        public int ExpiresIn { get; set; }

        public string Role { get; set; } = string.Empty;

        public string Username { get; set; } = string.Empty;
    }
}
