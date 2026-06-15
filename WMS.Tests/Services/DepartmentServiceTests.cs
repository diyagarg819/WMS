using Microsoft.Extensions.Logging;
using Moq;
using WMS.Application.Common;
using WMS.Application.DTOs.Department;
using WMS.Application.Services;
using WMS.Domain.Entities;
using WMS.Domain.Interfaces;

namespace WMS.Tests.Services
{
    public class DepartmentServiceTests
    {
        private readonly Mock<IDepartmentRepository> _mockRepository;
        private readonly Mock<ILogger<DepartmentService>> _mockLogger;
        private readonly DepartmentService _departmentService;

        public DepartmentServiceTests()
        {
            _mockRepository = new Mock<IDepartmentRepository>();
            _mockLogger = new Mock<ILogger<DepartmentService>>();
            _departmentService = new DepartmentService(_mockRepository.Object, _mockLogger.Object);
        }

        [Fact]
        public async Task GetAllDepartments_ReturnsList()
        {
            var departments = new List<Department>
            {
                new Department { DepartmentId = 1, DepartmentName = "HR", IsActive = true },
                new Department { DepartmentId = 2, DepartmentName = "IT", IsActive = true }
            };

            _mockRepository.Setup(r => r.GetAllAsync()).ReturnsAsync(departments);

            var request = new SearchRequestDto();
            var result = await _departmentService.GetAllDepartmentsAsync(request);

            Assert.Equal(2, result.Count);
            Assert.Equal("HR", result[0].DepartmentName);
        }

        [Fact]
        public async Task GetDepartmentById_WithValidId_ReturnsDepartment()
        {
            var dept = new Department { DepartmentId = 1, DepartmentName = "HR", IsActive = true };

            _mockRepository.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(dept);

            var result = await _departmentService.GetDepartmentByIdAsync(1);

            Assert.NotNull(result);
            Assert.Equal("HR", result.DepartmentName);
        }

        [Fact]
        public async Task GetDepartmentById_WithInvalidId_ReturnsNull()
        {
            _mockRepository.Setup(r => r.GetByIdAsync(999)).ReturnsAsync((Department?)null);

            var result = await _departmentService.GetDepartmentByIdAsync(999);

            Assert.Null(result);
        }

        [Fact]
        public async Task CreateDepartment_ReturnsDepartment()
        {
            var created = new Department { DepartmentId = 1, DepartmentName = "New Dept", IsActive = true };

            _mockRepository.Setup(r => r.AddAsync(It.IsAny<Department>(), It.IsAny<int>())).ReturnsAsync(created);

            var request = new CreateDepartmentDto { DepartmentName = "New Dept", IsActive = true };

            var result = await _departmentService.CreateDepartmentAsync(request, 1);

            Assert.NotNull(result);
            Assert.Equal("New Dept", result.DepartmentName);
        }

        [Fact]
        public async Task UpdateDepartment_WithValidData_ReturnsTrue()
        {
            var existing = new Department { DepartmentId = 1, DepartmentName = "Old", IsActive = true };

            _mockRepository.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(existing);
            _mockRepository.Setup(r => r.UpdateAsync(It.IsAny<Department>(), It.IsAny<int>())).Returns(Task.CompletedTask);

            var request = new UpdateDepartmentDto { DepartmentName = "Updated", IsActive = true };

            var result = await _departmentService.UpdateDepartmentAsync(1, request, 1);

            Assert.True(result);
        }

        [Fact]
        public async Task DeleteDepartment_WithValidId_ReturnsTrue()
        {
            var existing = new Department { DepartmentId = 1, DepartmentName = "Old", IsActive = true };

            _mockRepository.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(existing);
            _mockRepository.Setup(r => r.DeleteAsync(It.IsAny<Department>(), 1)).Returns(Task.CompletedTask);

            var result = await _departmentService.DeleteDepartmentAsync(1, 1);

            Assert.True(result);
        }
    }
}
