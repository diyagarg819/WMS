using WMS.Application.DTOs.Dashboard;

namespace WMS.Application.Services
{
    public interface IDashboardService
    {
        Task<(bool Success, string Message, DashboardResponseDto? Data)> GetDashboardDataAsync(int userId, string role);
    }
}
