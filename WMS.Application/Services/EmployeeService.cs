using Microsoft.Extensions.Logging;
using WMS.Application.Common;
using WMS.Application.DTOs.Employee;
using WMS.Domain.Entities;
using WMS.Domain.Interfaces;

namespace WMS.Application.Services
{
    /// <summary>
    /// Employee business logic — validation, mapping, and delegation to the repository.
    /// Never accesses the database directly — always goes through IEmployeeRepository.
    /// </summary>
    public class EmployeeService : IEmployeeService
    {
        private readonly IEmployeeRepository _employeeRepository;
        private readonly ILogger<EmployeeService> _logger;

        public EmployeeService(IEmployeeRepository employeeRepository, ILogger<EmployeeService> logger)
        {
            _employeeRepository = employeeRepository;
            _logger = logger;
        }

        // Get a paginated list of all employees
        public async Task<PagedResponseDto<EmployeeListDto>> GetAllEmployeesAsync(PagedRequestDto request)
        {
            var (employees, totalCount) = await _employeeRepository.GetAllAsync(
                request.PageNumber, request.PageSize, request.SearchTerm,
                request.SortBy, request.SortDirection);

            // Map entities to list DTOs
            var employeeList = employees.Select(e => MapToListDto(e)).ToList();

            return new PagedResponseDto<EmployeeListDto>
            {
                Data = employeeList,
                TotalCount = totalCount,
                PageNumber = request.PageNumber,
                PageSize = request.PageSize
            };
        }

        // Get a paginated list of employees in a specific department (Manager team view)
        public async Task<PagedResponseDto<EmployeeListDto>> GetEmployeesByDepartmentAsync(
            int departmentId, PagedRequestDto request)
        {
            var (employees, totalCount) = await _employeeRepository.GetByDepartmentAsync(
                departmentId, request.PageNumber, request.PageSize, request.SearchTerm);

            var employeeList = employees.Select(e => MapToListDto(e)).ToList();

            return new PagedResponseDto<EmployeeListDto>
            {
                Data = employeeList,
                TotalCount = totalCount,
                PageNumber = request.PageNumber,
                PageSize = request.PageSize
            };
        }

        // Get full details for a single employee
        public async Task<EmployeeDetailDto?> GetEmployeeByIdAsync(int employeeId)
        {
            var employee = await _employeeRepository.GetByIdAsync(employeeId);
            if (employee == null)
                return null;

            return MapToDetailDto(employee);
        }

        // Create a new employee — validates email uniqueness and age >= 18
        public async Task<EmployeeDetailDto?> CreateEmployeeAsync(CreateEmployeeDto request, int userId)
        {
            // Check if the email is already in use
            bool emailExists = await _employeeRepository.EmailExistsAsync(request.Email);
            if (emailExists)
            {
                _logger.LogWarning("Create employee failed — email already in use: {Email}", request.Email);
                return null;
            }

            // Validate that the employee is at least 18 years old
            int age = CalculateAge(request.DOB);
            if (age < 18)
            {
                _logger.LogWarning("Create employee failed — age is {Age}, must be at least 18", age);
                return null;
            }

            var employee = new Employee
            {
                FirstName = request.FirstName,
                LastName = request.LastName,
                Email = request.Email,
                PhoneNumber = request.PhoneNumber,
                Gender = request.Gender,
                DOB = request.DOB,
                DOJ = request.DOJ,
                DepartmentId = request.DepartmentId,
                RoleId = request.RoleId,
                Status = "Active",
                CreatedOn = DateTime.Now
            };

            var createdEmployee = await _employeeRepository.AddAsync(employee, userId);

            _logger.LogInformation("Employee created: {EmployeeId} — {FirstName} {LastName}",
                createdEmployee.EmployeeId, createdEmployee.FirstName, createdEmployee.LastName);

            // Re-fetch to include Department and Role navigation properties
            var result = await _employeeRepository.GetByIdAsync(createdEmployee.EmployeeId);
            return result != null ? MapToDetailDto(result) : null;
        }

        // Update an employee — Admin can update all fields
        public async Task<bool> UpdateEmployeeAsync(int employeeId, UpdateEmployeeDto request, int userId)
        {
            var employee = await _employeeRepository.GetByIdAsync(employeeId);
            if (employee == null)
                return false;

            // Check if the new email is already used by someone else
            bool emailExists = await _employeeRepository.EmailExistsAsync(request.Email, employeeId);
            if (emailExists)
            {
                _logger.LogWarning("Update employee failed — email already in use: {Email}", request.Email);
                return false;
            }

            // Validate age if DOB changed
            int age = CalculateAge(request.DOB);
            if (age < 18)
            {
                _logger.LogWarning("Update employee failed — age is {Age}, must be at least 18", age);
                return false;
            }

            // Apply the updates
            employee.FirstName = request.FirstName;
            employee.LastName = request.LastName;
            employee.Email = request.Email;
            employee.PhoneNumber = request.PhoneNumber;
            employee.Gender = request.Gender;
            employee.DOB = request.DOB;
            employee.DOJ = request.DOJ;
            employee.DepartmentId = request.DepartmentId;
            employee.RoleId = request.RoleId;
            employee.Status = request.Status ?? employee.Status;
            employee.UpdatedOn = DateTime.Now;

            await _employeeRepository.UpdateAsync(employee, userId);

            _logger.LogInformation("Employee updated: {EmployeeId}", employeeId);
            return true;
        }

        // Update own profile — employees can only change their PhoneNumber
        public async Task<bool> UpdateMyProfileAsync(int employeeId, UpdateMyProfileDto request, int userId)
        {
            var employee = await _employeeRepository.GetByIdAsync(employeeId);
            if (employee == null)
                return false;

            employee.PhoneNumber = request.PhoneNumber;
            employee.UpdatedOn = DateTime.Now;

            await _employeeRepository.UpdateAsync(employee, userId);

            _logger.LogInformation("Employee updated own profile: {EmployeeId}", employeeId);
            return true;
        }

        // Soft delete — marks as Inactive, does not remove from database
        public async Task<bool> DeleteEmployeeAsync(int employeeId, int deletedByUserId)
        {
            var employee = await _employeeRepository.GetByIdAsync(employeeId);
            if (employee == null)
                return false;

            await _employeeRepository.DeleteAsync(employee, deletedByUserId);

            _logger.LogInformation("Employee soft-deleted: {EmployeeId} by user: {DeletedBy}",
                employeeId, deletedByUserId);
            return true;
        }

        // Calculate age from date of birth
        private int CalculateAge(DateTime dateOfBirth)
        {
            var today = DateTime.Today;
            int age = today.Year - dateOfBirth.Year;

            // Subtract one if the birthday has not occurred yet this year
            if (dateOfBirth.Date > today.AddYears(-age))
                age--;

            return age;
        }

        // Map an Employee entity to the list DTO
        private EmployeeListDto MapToListDto(Employee employee)
        {
            return new EmployeeListDto
            {
                EmployeeId = employee.EmployeeId,
                FirstName = employee.FirstName,
                LastName = employee.LastName,
                Email = employee.Email,
                PhoneNumber = employee.PhoneNumber,
                DepartmentName = employee.Department?.DepartmentName,
                RoleName = employee.Role?.RoleName,
                Status = employee.Status
            };
        }

        // Map an Employee entity to the detail DTO
        private EmployeeDetailDto MapToDetailDto(Employee employee)
        {
            return new EmployeeDetailDto
            {
                EmployeeId = employee.EmployeeId,
                FirstName = employee.FirstName,
                LastName = employee.LastName,
                Email = employee.Email,
                PhoneNumber = employee.PhoneNumber,
                Gender = employee.Gender,
                DOB = employee.DOB,
                DOJ = employee.DOJ,
                DepartmentId = employee.DepartmentId,
                DepartmentName = employee.Department?.DepartmentName,
                RoleId = employee.RoleId,
                RoleName = employee.Role?.RoleName,
                Status = employee.Status,
                CreatedOn = employee.CreatedOn,
                UpdatedOn = employee.UpdatedOn
            };
        }
    }
}
