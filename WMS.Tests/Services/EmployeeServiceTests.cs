using Microsoft.Extensions.Logging;
using Moq;
using WMS.Application.Common;
using WMS.Application.DTOs.Employee;
using WMS.Application.Services;
using WMS.Domain.Entities;
using WMS.Domain.Interfaces;

namespace WMS.Tests.Services
{
    /// <summary>
    /// Unit tests for EmployeeService — covers CRUD, validation, and soft delete.
    /// </summary>
    public class EmployeeServiceTests
    {
        private readonly Mock<IEmployeeRepository> _mockRepository;
        private readonly Mock<ILogger<EmployeeService>> _mockLogger;
        private readonly EmployeeService _employeeService;

        public EmployeeServiceTests()
        {
            _mockRepository = new Mock<IEmployeeRepository>();
            _mockLogger = new Mock<ILogger<EmployeeService>>();
            _employeeService = new EmployeeService(_mockRepository.Object, _mockLogger.Object);
        }

        // ── GetAll Tests ──────────────────────────────────────────────────

        [Fact]
        public async Task GetAllEmployees_ReturnsPagedResult()
        {
            // Arrange — mock a page of 2 employees
            var employees = new List<Employee>
            {
                new Employee { EmployeeId = 1, FirstName = "John", LastName = "Doe", Email = "john@wms.com", PhoneNumber = "1234567890", Status = "Active" },
                new Employee { EmployeeId = 2, FirstName = "Jane", LastName = "Smith", Email = "jane@wms.com", PhoneNumber = "0987654321", Status = "Active" }
            };

            _mockRepository.Setup(r => r.GetAllAsync(1, 10, null, null, null))
                .ReturnsAsync((employees, 2));

            var request = new PagedRequestDto { PageNumber = 1, PageSize = 10 };

            // Act
            var result = await _employeeService.GetAllEmployeesAsync(request);

            // Assert
            Assert.Equal(2, result.TotalCount);
            Assert.Equal(2, result.Data.Count);
            Assert.Equal("John", result.Data[0].FirstName);
        }

        // ── GetById Tests ─────────────────────────────────────────────────

        [Fact]
        public async Task GetEmployeeById_WithValidId_ReturnsEmployee()
        {
            // Arrange
            var employee = new Employee
            {
                EmployeeId = 1, FirstName = "John", LastName = "Doe",
                Email = "john@wms.com", PhoneNumber = "123", Status = "Active",
                DOB = new DateTime(1990, 1, 1), DOJ = new DateTime(2020, 6, 1),
                Department = new Department { DepartmentId = 1, DepartmentName = "Engineering" },
                Role = new Role { RoleId = 3, RoleName = "Employee" }
            };

            _mockRepository.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(employee);

            // Act
            var result = await _employeeService.GetEmployeeByIdAsync(1);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("John", result.FirstName);
            Assert.Equal("Engineering", result.DepartmentName);
        }

        [Fact]
        public async Task GetEmployeeById_WithInvalidId_ReturnsNull()
        {
            _mockRepository.Setup(r => r.GetByIdAsync(999)).ReturnsAsync((Employee?)null);

            var result = await _employeeService.GetEmployeeByIdAsync(999);

            Assert.Null(result);
        }

        // ── Create Tests ──────────────────────────────────────────────────

        [Fact]
        public async Task CreateEmployee_WithValidData_ReturnsEmployee()
        {
            // Arrange — email is available and age >= 18
            _mockRepository.Setup(r => r.EmailExistsAsync("new@wms.com", null)).ReturnsAsync(false);

            var createdEmployee = new Employee
            {
                EmployeeId = 5, FirstName = "New", LastName = "User",
                Email = "new@wms.com", PhoneNumber = "555", Status = "Active",
                DOB = new DateTime(2000, 1, 1), DOJ = DateTime.Now
            };

            _mockRepository.Setup(r => r.AddAsync(It.IsAny<Employee>(), It.IsAny<int>())).ReturnsAsync(createdEmployee);
            _mockRepository.Setup(r => r.GetByIdAsync(5)).ReturnsAsync(createdEmployee);

            var request = new CreateEmployeeDto
            {
                FirstName = "New", LastName = "User",
                Email = "new@wms.com", PhoneNumber = "555",
                DOB = new DateTime(2000, 1, 1), DOJ = DateTime.Now
            };

            // Act
            var result = await _employeeService.CreateEmployeeAsync(request, 1);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("New", result.FirstName);
        }

        [Fact]
        public async Task CreateEmployee_WithDuplicateEmail_ReturnsNull()
        {
            // Arrange — email is already taken
            _mockRepository.Setup(r => r.EmailExistsAsync("taken@wms.com", null)).ReturnsAsync(true);

            var request = new CreateEmployeeDto
            {
                FirstName = "Test", LastName = "User",
                Email = "taken@wms.com", PhoneNumber = "555",
                DOB = new DateTime(2000, 1, 1), DOJ = DateTime.Now
            };

            // Act
            var result = await _employeeService.CreateEmployeeAsync(request, 1);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task CreateEmployee_UnderAge18_ReturnsNull()
        {
            // Arrange — employee is 15 years old
            _mockRepository.Setup(r => r.EmailExistsAsync("young@wms.com", null)).ReturnsAsync(false);

            var request = new CreateEmployeeDto
            {
                FirstName = "Young", LastName = "Person",
                Email = "young@wms.com", PhoneNumber = "555",
                DOB = DateTime.Today.AddYears(-15), DOJ = DateTime.Now
            };

            // Act
            var result = await _employeeService.CreateEmployeeAsync(request, 1);

            // Assert — under 18, should be rejected
            Assert.Null(result);
        }

        // ── Update Tests ──────────────────────────────────────────────────

        [Fact]
        public async Task UpdateEmployee_WithValidData_ReturnsTrue()
        {
            // Arrange
            var existing = new Employee
            {
                EmployeeId = 1, FirstName = "Old", LastName = "Name",
                Email = "old@wms.com", PhoneNumber = "123", Status = "Active",
                DOB = new DateTime(1990, 1, 1), DOJ = new DateTime(2020, 1, 1)
            };

            _mockRepository.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(existing);
            _mockRepository.Setup(r => r.EmailExistsAsync("new@wms.com", 1)).ReturnsAsync(false);
            _mockRepository.Setup(r => r.UpdateAsync(It.IsAny<Employee>(), It.IsAny<int>())).Returns(Task.CompletedTask);

            var request = new UpdateEmployeeDto
            {
                FirstName = "Updated", LastName = "Name",
                Email = "new@wms.com", PhoneNumber = "456",
                DOB = new DateTime(1990, 1, 1), DOJ = new DateTime(2020, 1, 1)
            };

            // Act
            var result = await _employeeService.UpdateEmployeeAsync(1, request, 1);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public async Task UpdateEmployee_NotFound_ReturnsFalse()
        {
            _mockRepository.Setup(r => r.GetByIdAsync(999)).ReturnsAsync((Employee?)null);

            var request = new UpdateEmployeeDto
            {
                FirstName = "Test", LastName = "User",
                Email = "test@wms.com", PhoneNumber = "123",
                DOB = new DateTime(1990, 1, 1), DOJ = new DateTime(2020, 1, 1)
            };

            var result = await _employeeService.UpdateEmployeeAsync(999, request, 1);

            Assert.False(result);
        }

        // ── Delete Tests ──────────────────────────────────────────────────

        [Fact]
        public async Task DeleteEmployee_WithValidId_ReturnsTrue()
        {
            var employee = new Employee
            {
                EmployeeId = 1, FirstName = "John", LastName = "Doe",
                Email = "john@wms.com", PhoneNumber = "123", Status = "Active",
                DOB = new DateTime(1990, 1, 1), DOJ = new DateTime(2020, 1, 1)
            };

            _mockRepository.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(employee);
            _mockRepository.Setup(r => r.DeleteAsync(It.IsAny<Employee>(), 1)).Returns(Task.CompletedTask);

            var result = await _employeeService.DeleteEmployeeAsync(1, 1);

            Assert.True(result);
        }

        [Fact]
        public async Task DeleteEmployee_NotFound_ReturnsFalse()
        {
            _mockRepository.Setup(r => r.GetByIdAsync(999)).ReturnsAsync((Employee?)null);

            var result = await _employeeService.DeleteEmployeeAsync(999, 1);

            Assert.False(result);
        }

        // ── MyProfile Tests ───────────────────────────────────────────────

        [Fact]
        public async Task UpdateMyProfile_WithValidData_ReturnsTrue()
        {
            var employee = new Employee
            {
                EmployeeId = 1, FirstName = "John", LastName = "Doe",
                Email = "john@wms.com", PhoneNumber = "123", Status = "Active",
                DOB = new DateTime(1990, 1, 1), DOJ = new DateTime(2020, 1, 1)
            };

            _mockRepository.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(employee);
            _mockRepository.Setup(r => r.UpdateAsync(It.IsAny<Employee>(), It.IsAny<int>())).Returns(Task.CompletedTask);

            var request = new UpdateMyProfileDto { PhoneNumber = "9999999999" };

            var result = await _employeeService.UpdateMyProfileAsync(1, request, 1);

            Assert.True(result);
        }
    }
}
