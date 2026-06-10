using Microsoft.Extensions.Logging;
using Moq;
using WMS.Application.DTOs.Attendance;
using WMS.Application.Services;
using WMS.Domain.Entities;
using WMS.Domain.Interfaces;

namespace WMS.Tests.Services
{
    /// <summary>
    /// Unit tests for AttendanceService — covers check-in, check-out, duplicate prevention, and history.
    /// </summary>
    public class AttendanceServiceTests
    {
        private readonly Mock<IAttendanceRepository> _mockRepository;
        private readonly Mock<ILogger<AttendanceService>> _mockLogger;
        private readonly AttendanceService _attendanceService;

        public AttendanceServiceTests()
        {
            _mockRepository = new Mock<IAttendanceRepository>();
            _mockLogger = new Mock<ILogger<AttendanceService>>();
            _attendanceService = new AttendanceService(_mockRepository.Object, _mockLogger.Object);
        }

        // ── Check-In Tests ────────────────────────────────────────────────

        [Fact]
        public async Task CheckIn_FirstTimeToday_ReturnsRecord()
        {
            // Arrange — no existing record for today
            _mockRepository.Setup(r => r.GetTodayRecordAsync(1)).ReturnsAsync((Attendance?)null);
            _mockRepository.Setup(r => r.AddAsync(It.IsAny<Attendance>()))
                .ReturnsAsync((Attendance a) => { a.AttendanceId = 1; return a; });

            var request = new CheckInDto { WorkMode = "WFO" };

            // Act
            var result = await _attendanceService.CheckInAsync(1, request);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("WFO", result.WorkMode);
            _mockRepository.Verify(r => r.AddAsync(It.IsAny<Attendance>()), Times.Once);
        }

        [Fact]
        public async Task CheckIn_AlreadyCheckedInToday_ReturnsNull()
        {
            // Arrange — record already exists for today
            var existing = new Attendance
            {
                AttendanceId = 1, EmpId = 1,
                CheckIn = DateTime.Today.AddHours(9),
                AttendanceDate = DateTime.Today
            };

            _mockRepository.Setup(r => r.GetTodayRecordAsync(1)).ReturnsAsync(existing);

            var request = new CheckInDto { WorkMode = "WFH" };

            // Act
            var result = await _attendanceService.CheckInAsync(1, request);

            // Assert — duplicate check-in should be rejected
            Assert.Null(result);
        }

        // ── Check-Out Tests ───────────────────────────────────────────────

        [Fact]
        public async Task CheckOut_WithPendingCheckIn_ReturnsRecord()
        {
            // Arrange — checked in but not yet checked out
            var record = new Attendance
            {
                AttendanceId = 1, EmpId = 1,
                CheckIn = DateTime.Today.AddHours(9),
                CheckOut = null,
                AttendanceDate = DateTime.Today
            };

            _mockRepository.Setup(r => r.GetTodayRecordAsync(1)).ReturnsAsync(record);
            _mockRepository.Setup(r => r.UpdateAsync(It.IsAny<Attendance>())).Returns(Task.CompletedTask);
            _mockRepository.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(record);

            // Act
            var result = await _attendanceService.CheckOutAsync(1);

            // Assert
            Assert.NotNull(result);
            _mockRepository.Verify(r => r.UpdateAsync(It.IsAny<Attendance>()), Times.Once);
        }

        [Fact]
        public async Task CheckOut_NotCheckedInToday_ReturnsNull()
        {
            // Arrange — no record exists for today
            _mockRepository.Setup(r => r.GetTodayRecordAsync(1)).ReturnsAsync((Attendance?)null);

            // Act
            var result = await _attendanceService.CheckOutAsync(1);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task CheckOut_AlreadyCheckedOut_ReturnsNull()
        {
            // Arrange — already checked out
            var record = new Attendance
            {
                AttendanceId = 1, EmpId = 1,
                CheckIn = DateTime.Today.AddHours(9),
                CheckOut = DateTime.Today.AddHours(17),
                AttendanceDate = DateTime.Today
            };

            _mockRepository.Setup(r => r.GetTodayRecordAsync(1)).ReturnsAsync(record);

            // Act
            var result = await _attendanceService.CheckOutAsync(1);

            // Assert — should not allow double check-out
            Assert.Null(result);
        }

        // ── Today Status Tests ────────────────────────────────────────────

        [Fact]
        public async Task GetTodayStatus_WithRecord_ReturnsRecord()
        {
            var record = new Attendance
            {
                AttendanceId = 1, EmpId = 1,
                CheckIn = DateTime.Today.AddHours(9),
                AttendanceDate = DateTime.Today
            };

            _mockRepository.Setup(r => r.GetTodayRecordAsync(1)).ReturnsAsync(record);

            var result = await _attendanceService.GetTodayStatusAsync(1);

            Assert.NotNull(result);
        }

        [Fact]
        public async Task GetTodayStatus_NoRecord_ReturnsNull()
        {
            _mockRepository.Setup(r => r.GetTodayRecordAsync(1)).ReturnsAsync((Attendance?)null);

            var result = await _attendanceService.GetTodayStatusAsync(1);

            Assert.Null(result);
        }

        // ── History Tests ─────────────────────────────────────────────────

        [Fact]
        public async Task GetMyAttendance_ReturnsPaginatedRecords()
        {
            var records = new List<Attendance>
            {
                new Attendance { AttendanceId = 1, EmpId = 1, CheckIn = DateTime.Today.AddHours(9), AttendanceDate = DateTime.Today },
                new Attendance { AttendanceId = 2, EmpId = 1, CheckIn = DateTime.Today.AddDays(-1).AddHours(9), AttendanceDate = DateTime.Today.AddDays(-1) }
            };

            _mockRepository.Setup(r => r.GetByEmployeeAsync(1, 1, 10, null, null))
                .ReturnsAsync((records, 2));

            var filter = new AttendanceFilterDto { PageNumber = 1, PageSize = 10 };

            var result = await _attendanceService.GetMyAttendanceAsync(1, filter);

            Assert.Equal(2, result.TotalCount);
            Assert.Equal(2, result.Data.Count);
        }
    }
}
