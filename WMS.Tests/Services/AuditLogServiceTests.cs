using Microsoft.Extensions.Logging;
using Moq;
using WMS.Application.Common;
using WMS.Application.DTOs.AuditLog;
using WMS.Application.Services;
using WMS.Domain.Entities;
using WMS.Domain.Interfaces;

namespace WMS.Tests.Services
{
    public class AuditLogServiceTests
    {
        private readonly Mock<IAuditLogRepository> _mockRepository;
        private readonly Mock<ILogger<AuditLogService>> _mockLogger;
        private readonly AuditLogService _auditLogService;

        public AuditLogServiceTests()
        {
            _mockRepository = new Mock<IAuditLogRepository>();
            _mockLogger = new Mock<ILogger<AuditLogService>>();
            _auditLogService = new AuditLogService(_mockRepository.Object, _mockLogger.Object);
        }

        [Fact]
        public async Task GetAuditLogs_ReturnsList()
        {
            var logs = new List<AuditLog>
            {
                new AuditLog { AuditId = 1, Action = "Create", EntityName = "Employee", EntityId = 1, Details = "Created", PerformedBy = 1, PerformedAt = DateTime.Now },
                new AuditLog { AuditId = 2, Action = "Update", EntityName = "Employee", EntityId = 1, Details = "Updated", PerformedBy = 1, PerformedAt = DateTime.Now }
            };

            _mockRepository.Setup(r => r.GetLogsAsync(It.IsAny<DateTime?>(), It.IsAny<DateTime?>())).ReturnsAsync(logs);

            var request = new SearchRequestDto();
            var result = await _auditLogService.GetAuditLogsAsync(request, null, null);

            Assert.Equal(2, result.Count);
            Assert.Equal("Create", result[0].Action);
        }
    }
}
