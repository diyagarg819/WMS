using Microsoft.Extensions.Logging;
using Moq;
using WMS.Application.Common;
using WMS.Application.DTOs.Leave;
using WMS.Application.Services;
using WMS.Domain.Entities;
using WMS.Domain.Interfaces;

namespace WMS.Tests.Services
{
    public class LeaveServiceTests
    {
        private readonly Mock<ILeaveRepository> _mockRepository;
        private readonly Mock<ILogger<LeaveService>> _mockLogger;
        private readonly LeaveService _leaveService;

        public LeaveServiceTests()
        {
            _mockRepository = new Mock<ILeaveRepository>();
            _mockLogger = new Mock<ILogger<LeaveService>>();
            _leaveService = new LeaveService(_mockRepository.Object, _mockLogger.Object);
        }

        [Fact]
        public async Task GetAllLeaves_ReturnsList()
        {
            var leaves = new List<Leave>
            {
                new Leave { LeaveId = 1, EmployeeId = 1, StartDate = DateTime.Now, EndDate = DateTime.Now.AddDays(1), Status = "Pending" }
            };

            _mockRepository.Setup(r => r.GetAllAsync(null)).ReturnsAsync(leaves);

            var request = new SearchRequestDto();
            var result = await _leaveService.GetAllLeavesAsync(request);

            Assert.Single(result);
            Assert.Equal("Pending", result[0].Status);
        }

        [Fact]
        public async Task ApplyLeave_WithNoOverlap_ReturnsLeave()
        {
            var newLeave = new Leave { LeaveId = 1, EmployeeId = 1, StartDate = DateTime.Now, EndDate = DateTime.Now.AddDays(1), Status = "Pending" };

            _mockRepository.Setup(r => r.HasOverlappingLeaveAsync(It.IsAny<int>(), It.IsAny<DateTime>(), It.IsAny<DateTime>(), null)).ReturnsAsync(false);
            _mockRepository.Setup(r => r.AddAsync(It.IsAny<Leave>(), It.IsAny<int>())).ReturnsAsync(newLeave);

            var request = new ApplyLeaveDto { StartDate = DateTime.Now, EndDate = DateTime.Now.AddDays(1), Reason = "Sick" };
            var result = await _leaveService.ApplyLeaveAsync(1, request);

            Assert.NotNull(result);
        }

        [Fact]
        public async Task ApplyLeave_WithOverlap_ReturnsNull()
        {
            _mockRepository.Setup(r => r.HasOverlappingLeaveAsync(It.IsAny<int>(), It.IsAny<DateTime>(), It.IsAny<DateTime>(), null)).ReturnsAsync(true);

            var request = new ApplyLeaveDto { StartDate = DateTime.Now, EndDate = DateTime.Now.AddDays(1), Reason = "Sick" };
            var result = await _leaveService.ApplyLeaveAsync(1, request);

            Assert.Null(result); // Should fail due to overlap
        }

        [Fact]
        public async Task UpdateLeaveStatus_WithValidData_ReturnsTrue()
        {
            var existing = new Leave { LeaveId = 1, EmployeeId = 1, StartDate = DateTime.Now, EndDate = DateTime.Now.AddDays(1), Status = "Pending" };

            _mockRepository.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(existing);
            _mockRepository.Setup(r => r.UpdateAsync(It.IsAny<Leave>(), It.IsAny<int>())).Returns(Task.CompletedTask);

            var request = new UpdateLeaveStatusDto { Status = "Approved", ApproverComment = "OK" };
            var result = await _leaveService.UpdateLeaveStatusAsync(1, request, 2);

            Assert.True(result);
            Assert.Equal("Approved", existing.Status);
        }
    }
}
