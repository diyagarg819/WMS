using Microsoft.Extensions.Logging;
using WMS.Application.DTOs.Dashboard;
using WMS.Domain.Interfaces;

namespace WMS.Application.Services
{
    public class DashboardService : IDashboardService
    {
        private readonly IDashboardRepository _dashboardRepository;
        private readonly IEmployeeRepository _employeeRepository;
        private readonly ILogger<DashboardService> _logger;

        public DashboardService(
            IDashboardRepository dashboardRepository,
            IEmployeeRepository employeeRepository,
            ILogger<DashboardService> logger)
        {
            _dashboardRepository = dashboardRepository;
            _employeeRepository = employeeRepository;
            _logger = logger;
        }

        public async Task<(bool Success, string Message, DashboardResponseDto? Data)> GetDashboardDataAsync(int userId, string role)
        {
            // Lookup employee to find their department for Manager-scoped queries
            var employee = await _employeeRepository.GetByIdAsync(userId);
            int? departmentId = employee?.DepartmentId;

            var kpis = new DashboardKpiDto();
            var charts = new List<DashboardChartDto>();

            // 1. Employee Count (Admin only sees this count)
            if (role == "Admin")
            {
                kpis.TotalEmployees = await _dashboardRepository.GetTotalEmployeeCountAsync();
            }

            // 2. Attendance Count Today
            kpis.AttendanceCountToday = await _dashboardRepository.GetAttendanceCountTodayAsync(role, userId, departmentId);

            // 3. Pending Leave Count
            kpis.PendingLeaveCount = await _dashboardRepository.GetPendingLeaveCountAsync(role, userId, departmentId);

            // 4. Active Project Count
            kpis.ActiveProjectCount = await _dashboardRepository.GetActiveProjectCountAsync(role, userId);

            // 5. Chart: Attendance This Month
            DateTime today = DateTime.Today;
            DateTime startOfMonth = new DateTime(today.Year, today.Month, 1);
            
            var attendanceData = await _dashboardRepository.GetAttendanceChartDataAsync(role, userId, departmentId, startOfMonth, today);
            charts.Add(new DashboardChartDto
            {
                ChartName = "AttendanceThisMonth",
                Labels = attendanceData.Select(x => x.Key).ToList(),
                Data = attendanceData.Select(x => x.Value).ToList()
            });

            // 6. Chart: Leave Breakdown
            var leaveData = await _dashboardRepository.GetLeavePieChartDataAsync(role, userId, departmentId);
            charts.Add(new DashboardChartDto
            {
                ChartName = "LeaveBreakdown",
                Labels = leaveData.Select(x => x.Key).ToList(),
                Data = leaveData.Select(x => x.Value).ToList()
            });

            // 7. Chart: Project Status
            var projectData = await _dashboardRepository.GetProjectBarChartDataAsync(role, userId);
            charts.Add(new DashboardChartDto
            {
                ChartName = "ProjectStatus",
                Labels = projectData.Select(x => x.Key).ToList(),
                Data = projectData.Select(x => x.Value).ToList()
            });

            // 8. Announcements (Top 5 most recent active)
            var announcementsRaw = await _dashboardRepository.GetLatestAnnouncementsAsync(5);
            var announcements = announcementsRaw.Select(a => new AnnouncementDto
            {
                AnnouncementId = a.AnnouncementId,
                Title = a.Title,
                Message = a.Message,
                CreatedOn = a.CreatedOn,
                CreatorName = a.Creator != null ? $"{a.Creator.FirstName} {a.Creator.LastName}" : "System"
            }).ToList();

            var response = new DashboardResponseDto
            {
                Kpis = kpis,
                Charts = charts,
                Announcements = announcements
            };

            return (true, "Dashboard data retrieved successfully.", response);
        }
    }
}
