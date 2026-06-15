using Microsoft.Extensions.Logging;
using Moq;
using WMS.Application.Common;
using WMS.Application.DTOs.Project;
using WMS.Application.Services;
using WMS.Domain.Entities;
using WMS.Domain.Interfaces;

namespace WMS.Tests.Services
{
    public class ProjectServiceTests
    {
        private readonly Mock<IProjectRepository> _mockRepository;
        private readonly Mock<ILogger<ProjectService>> _mockLogger;
        private readonly ProjectService _projectService;

        public ProjectServiceTests()
        {
            _mockRepository = new Mock<IProjectRepository>();
            _mockLogger = new Mock<ILogger<ProjectService>>();
            _projectService = new ProjectService(_mockRepository.Object, _mockLogger.Object);
        }

        [Fact]
        public async Task GetAllProjects_ReturnsList()
        {
            var projects = new List<Project>
            {
                new Project { ProjectId = 1, ProjectName = "Proj 1", Status = "Active" }
            };

            _mockRepository.Setup(r => r.GetAllAsync()).ReturnsAsync(projects);

            var request = new SearchRequestDto();
            var result = await _projectService.GetAllProjectsAsync(request);

            Assert.Single(result);
            Assert.Equal("Proj 1", result[0].ProjectName);
        }

        [Fact]
        public async Task GetProjectById_WithValidId_ReturnsProject()
        {
            var project = new Project { ProjectId = 1, ProjectName = "Proj 1", Status = "Active" };

            _mockRepository.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(project);

            var result = await _projectService.GetProjectByIdAsync(1);

            Assert.NotNull(result);
            Assert.Equal("Proj 1", result.ProjectName);
        }

        [Fact]
        public async Task CreateProject_ReturnsProject()
        {
            var created = new Project { ProjectId = 1, ProjectName = "New", Status = "Active" };

            _mockRepository.Setup(r => r.AddAsync(It.IsAny<Project>(), It.IsAny<int>())).ReturnsAsync(created);

            var request = new CreateProjectDto { ProjectName = "New", Status = "Active", ClientId = 1 };

            var result = await _projectService.CreateProjectAsync(request, 1);

            Assert.NotNull(result);
            Assert.Equal("New", result.ProjectName);
        }

        [Fact]
        public async Task AllocateEmployee_WithValidData_ReturnsTrue()
        {
            _mockRepository.Setup(r => r.AddAllocationAsync(It.IsAny<ProjectAllocation>(), It.IsAny<int>())).Returns(Task.CompletedTask);

            var request = new AllocateEmployeeDto { EmployeeId = 1, AllocationPercentage = 100, StartDate = DateTime.Now };

            var result = await _projectService.AllocateEmployeeAsync(1, request, 1);

            Assert.True(result);
        }

        [Fact]
        public async Task DeallocateEmployee_WithValidData_ReturnsTrue()
        {
            var allocation = new ProjectAllocation { AllocationId = 1, ProjectId = 1, EmployeeId = 1, IsActive = true };

            _mockRepository.Setup(r => r.GetAllocationByIdAsync(1)).ReturnsAsync(allocation);
            _mockRepository.Setup(r => r.UpdateAllocationAsync(It.IsAny<ProjectAllocation>(), It.IsAny<int>())).Returns(Task.CompletedTask);

            var request = new DeallocateEmployeeDto { EndDate = DateTime.Now };

            var result = await _projectService.DeallocateEmployeeAsync(1, 1, request, 1);

            Assert.True(result);
            Assert.False(allocation.IsActive);
        }
    }
}
