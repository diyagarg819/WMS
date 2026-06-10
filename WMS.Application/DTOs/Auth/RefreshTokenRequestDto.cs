using System.ComponentModel.DataAnnotations;

namespace WMS.Application.DTOs.Auth
{
    /// <summary>
    /// Request body for the token refresh endpoint.
    /// </summary>
    public class RefreshTokenRequestDto
    {
        [Required(ErrorMessage = "Refresh token is required")]
        public string RefreshToken { get; set; } = string.Empty;
    }
}
