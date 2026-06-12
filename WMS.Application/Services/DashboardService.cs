using WMS.Application.DTOs.Dashboard;
using WMS.Domain.Interfaces;

namespace WMS.Application.Services
{
    public class DashboardService : IDashboardService
    {
        private readonly IDashboardRepository _dashboardRepository;
        private readonly IEmployeeRepository _employeeRepository;

        public DashboardService(
            IDashboardRepository dashboardRepository,
            IEmployeeRepository employeeRepository)
        {
            _dashboardRepository = dashboardRepository;
            _employeeRepository = employeeRepository;
        }

        public async Task<(bool Success, string Message, DashboardResponseDto? Data)> GetDashboardDataAsync(int userId, string role)
        {
            var employee = await _employeeRepository.GetByIdAsync(userId);
            int? departmentId = employee?.DepartmentId;

            var kpis = new DashboardKpiDto();
            var charts = new List<DashboardChartDto>();

            if (role == "Admin")
            {
                kpis.TotalEmployees = await _dashboardRepository.GetTotalEmployeeCountAsync();
            }

            kpis.AttendanceCountToday = await _dashboardRepository.GetAttendanceCountTodayAsync(role, userId, departmentId);
            kpis.PendingLeaveCount = await _dashboardRepository.GetPendingLeaveCountAsync(role, userId, departmentId);
            kpis.LeavesTaken = await _dashboardRepository.GetLeavesTakenCountAsync(role, userId, departmentId);
            kpis.ActiveProjectCount = await _dashboardRepository.GetActiveProjectCountAsync(role, userId);
            kpis.TotalProjects = await _dashboardRepository.GetTotalProjectsCountAsync(role, userId);
            kpis.AllocatedEmployees = await _dashboardRepository.GetAllocatedEmployeesCountAsync(role, userId);

            if (role == "Admin")
            {
                var leaveData = await _dashboardRepository.GetLeavePieChartDataAsync(role, userId, departmentId);
                charts.Add(new DashboardChartDto
                {
                    ChartName = "LeaveStatus",
                    Labels = leaveData.Select(x => x.Key).ToList(),
                    Data = leaveData.Select(x => x.Value).ToList()
                });
            }
            else
            {
                var attendanceData = await _dashboardRepository.GetMonthlyAttendanceBarChartDataAsync(role, userId, departmentId);
                charts.Add(new DashboardChartDto
                {
                    ChartName = "MonthlyAttendance",
                    Labels = attendanceData.Select(x => x.Key).ToList(),
                    Data = attendanceData.Select(x => x.Value).ToList()
                });
            }

            var projectData = await _dashboardRepository.GetProjectBarChartDataAsync(role, userId);
            charts.Add(new DashboardChartDto
            {
                ChartName = "ProjectStatus",
                Labels = projectData.Select(x => x.Key).ToList(),
                Data = projectData.Select(x => x.Value).ToList()
            });

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
