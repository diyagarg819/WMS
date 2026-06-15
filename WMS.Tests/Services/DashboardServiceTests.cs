using Microsoft.Extensions.Logging;
using Moq;
using WMS.Application.DTOs.Dashboard;
using WMS.Application.Services;
using WMS.Domain.Entities;
using WMS.Domain.Interfaces;

namespace WMS.Tests.Services
{
    public class DashboardServiceTests
    {
        private readonly Mock<IEmployeeRepository> _mockEmployeeRepo;
        private readonly Mock<IAttendanceRepository> _mockAttendanceRepo;
        private readonly Mock<ILeaveRepository> _mockLeaveRepo;
        private readonly Mock<IProjectRepository> _mockProjectRepo;
        private readonly Mock<IAnnouncementRepository> _mockAnnouncementRepo;
        private readonly Mock<ILogger<DashboardService>> _mockLogger;
        private readonly DashboardService _dashboardService;

        public DashboardServiceTests()
        {
            _mockEmployeeRepo = new Mock<IEmployeeRepository>();
            _mockAttendanceRepo = new Mock<IAttendanceRepository>();
            _mockLeaveRepo = new Mock<ILeaveRepository>();
            _mockProjectRepo = new Mock<IProjectRepository>();
            _mockAnnouncementRepo = new Mock<IAnnouncementRepository>();
            _mockLogger = new Mock<ILogger<DashboardService>>();

            _dashboardService = new DashboardService(
                _mockEmployeeRepo.Object,
                _mockAttendanceRepo.Object,
                _mockLeaveRepo.Object,
                _mockProjectRepo.Object,
                _mockAnnouncementRepo.Object,
                _mockLogger.Object
            );
        }

        [Fact]
        public async Task GetAdminDashboard_ReturnsData()
        {
            _mockEmployeeRepo.Setup(r => r.GetAllAsync(null)).ReturnsAsync(new List<Employee> { new Employee() });
            _mockAttendanceRepo.Setup(r => r.GetAttendanceByDateAsync(It.IsAny<DateTime>())).ReturnsAsync(new List<Attendance> { new Attendance() });
            _mockLeaveRepo.Setup(r => r.GetLeavesByDateRangeAsync(It.IsAny<DateTime>(), It.IsAny<DateTime>())).ReturnsAsync(new List<Leave>());
            _mockProjectRepo.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<Project> { new Project { Status = "Active" } });
            _mockProjectRepo.Setup(r => r.GetAllAllocationsAsync()).ReturnsAsync(new List<ProjectAllocation>());
            _mockAnnouncementRepo.Setup(r => r.GetRecentAnnouncementsAsync(It.IsAny<int>(), null)).ReturnsAsync(new List<Announcement>());

            var result = await _dashboardService.GetDashboardDataAsync(1, "Admin");

            Assert.NotNull(result);
            Assert.Equal(1, result.Kpis.TotalEmployees);
            Assert.Equal(1, result.Kpis.AttendanceCountToday);
            Assert.Equal(1, result.Kpis.ActiveProjectCount);
            Assert.NotNull(result.Charts);
        }

        [Fact]
        public async Task GetEmployeeDashboard_ReturnsData()
        {
            _mockLeaveRepo.Setup(r => r.GetLeavesByEmployeeAsync(1)).ReturnsAsync(new List<Leave> { new Leave { Status = "Approved" } });
            _mockProjectRepo.Setup(r => r.GetAllocationsByEmployeeAsync(1)).ReturnsAsync(new List<ProjectAllocation> { new ProjectAllocation() });
            _mockAnnouncementRepo.Setup(r => r.GetRecentAnnouncementsAsync(It.IsAny<int>(), 1)).ReturnsAsync(new List<Announcement>());

            var result = await _dashboardService.GetDashboardDataAsync(1, "Employee");

            Assert.NotNull(result);
            Assert.Equal(1, result.Kpis.LeavesTaken);
            Assert.Equal(1, result.Kpis.ActiveProjectCount); // Allocated projects for employee
            Assert.NotNull(result.Charts);
        }
    }
}
