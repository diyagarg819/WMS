using Microsoft.Extensions.Logging;
using Moq;
using WMS.Application.Common;
using WMS.Application.DTOs.Announcement;
using WMS.Application.Services;
using WMS.Domain.Entities;
using WMS.Domain.Interfaces;

namespace WMS.Tests.Services
{
    public class AnnouncementServiceTests
    {
        private readonly Mock<IAnnouncementRepository> _mockRepository;
        private readonly Mock<ILogger<AnnouncementService>> _mockLogger;
        private readonly AnnouncementService _announcementService;

        public AnnouncementServiceTests()
        {
            _mockRepository = new Mock<IAnnouncementRepository>();
            _mockLogger = new Mock<ILogger<AnnouncementService>>();
            _announcementService = new AnnouncementService(_mockRepository.Object, _mockLogger.Object);
        }

        [Fact]
        public async Task GetAllAnnouncements_ReturnsList()
        {
            var announcements = new List<Announcement>
            {
                new Announcement { AnnouncementId = 1, Title = "Test 1", Message = "Msg 1", IsActive = true, CreatedBy = 1, CreatedOn = DateTime.Now },
                new Announcement { AnnouncementId = 2, Title = "Test 2", Message = "Msg 2", IsActive = true, CreatedBy = 1, CreatedOn = DateTime.Now }
            };

            _mockRepository.Setup(r => r.GetAllActiveAsync(It.IsAny<int?>())).ReturnsAsync(announcements);

            var request = new SearchRequestDto();
            var result = await _announcementService.GetAllAnnouncementsAsync(request, null);

            Assert.Equal(2, result.Count);
            Assert.Equal("Test 1", result[0].Title);
        }

        [Fact]
        public async Task GetAnnouncementById_WithValidId_ReturnsAnnouncement()
        {
            var announcement = new Announcement { AnnouncementId = 1, Title = "Test 1", Message = "Msg 1", IsActive = true, CreatedBy = 1, CreatedOn = DateTime.Now };

            _mockRepository.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(announcement);

            var result = await _announcementService.GetAnnouncementByIdAsync(1);

            Assert.NotNull(result);
            Assert.Equal("Test 1", result.Title);
        }

        [Fact]
        public async Task GetAnnouncementById_WithInvalidId_ReturnsNull()
        {
            _mockRepository.Setup(r => r.GetByIdAsync(999)).ReturnsAsync((Announcement?)null);

            var result = await _announcementService.GetAnnouncementByIdAsync(999);

            Assert.Null(result);
        }

        [Fact]
        public async Task CreateAnnouncement_ReturnsAnnouncement()
        {
            var created = new Announcement { AnnouncementId = 1, Title = "New", Message = "Msg", IsActive = true, CreatedBy = 1, CreatedOn = DateTime.Now };

            _mockRepository.Setup(r => r.AddAsync(It.IsAny<Announcement>(), It.IsAny<int>())).ReturnsAsync(created);

            var request = new CreateAnnouncementDto { Title = "New", Message = "Msg", TargetAudience = "All", IsActive = true };

            var result = await _announcementService.CreateAnnouncementAsync(request, 1);

            Assert.NotNull(result);
            Assert.Equal("New", result.Title);
        }

        [Fact]
        public async Task UpdateAnnouncement_WithValidData_ReturnsTrue()
        {
            var existing = new Announcement { AnnouncementId = 1, Title = "Old", Message = "Old msg", IsActive = true, CreatedBy = 1, CreatedOn = DateTime.Now };

            _mockRepository.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(existing);
            _mockRepository.Setup(r => r.UpdateAsync(It.IsAny<Announcement>(), It.IsAny<int>())).Returns(Task.CompletedTask);

            var request = new UpdateAnnouncementDto { Title = "Updated", Message = "Msg", TargetAudience = "All", IsActive = true };

            var result = await _announcementService.UpdateAnnouncementAsync(1, request, 1);

            Assert.True(result);
        }

        [Fact]
        public async Task DeleteAnnouncement_WithValidId_ReturnsTrue()
        {
            var existing = new Announcement { AnnouncementId = 1, Title = "Old", Message = "Old msg", IsActive = true, CreatedBy = 1, CreatedOn = DateTime.Now };

            _mockRepository.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(existing);
            _mockRepository.Setup(r => r.DeleteAsync(It.IsAny<Announcement>(), 1)).Returns(Task.CompletedTask);

            var result = await _announcementService.DeleteAnnouncementAsync(1, 1);

            Assert.True(result);
        }
    }
}
