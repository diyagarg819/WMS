using WMS.Application.DTOs.Auth;

namespace WMS.Application.Services
{
    public interface IAuthService
    {
        Task<LoginResponseDto?> LoginAsync(LoginRequestDto request);
    }
}
