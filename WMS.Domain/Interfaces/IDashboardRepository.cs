using WMS.Domain.Entities;

namespace WMS.Domain.Interfaces
{
    public interface IDashboardRepository
    {
        Task<int> GetTotalEmployeeCountAsync();
        
        Task<int> GetAttendanceCountTodayAsync(string role, int employeeId, int? departmentId);
        
        Task<int> GetPendingLeaveCountAsync(string role, int employeeId, int? departmentId);
        
        Task<int> GetActiveProjectCountAsync(string role, int employeeId);

        // Chart Data
        Task<List<KeyValuePair<string, int>>> GetAttendanceChartDataAsync(string role, int employeeId, int? departmentId, DateTime startDate, DateTime endDate);
        
        Task<List<KeyValuePair<string, int>>> GetLeavePieChartDataAsync(string role, int employeeId, int? departmentId);
        
        Task<List<KeyValuePair<string, int>>> GetProjectBarChartDataAsync(string role, int employeeId);

        Task<List<Announcement>> GetLatestAnnouncementsAsync(int count);
    }
}
