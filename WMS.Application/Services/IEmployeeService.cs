using WMS.Application.Common;
using WMS.Application.DTOs.Employee;

namespace WMS.Application.Services
{
    /// <summary>
    /// Service interface for Employee business operations.
    /// </summary>
    public interface IEmployeeService
    {
        // Get a paginated list of all employees (Admin, Manager)
        Task<PagedResponseDto<EmployeeListDto>> GetAllEmployeesAsync(PagedRequestDto request);

        // Get a paginated list of employees within a department (Manager team-scoped)
        Task<PagedResponseDto<EmployeeListDto>> GetEmployeesByDepartmentAsync(
            int departmentId, PagedRequestDto request);

        // Get a single employee by ID
        Task<EmployeeDetailDto?> GetEmployeeByIdAsync(int employeeId);

        // Create a new employee (Admin only)
        Task<EmployeeDetailDto?> CreateEmployeeAsync(CreateEmployeeDto request);

        // Update an employee — full update (Admin only)
        Task<bool> UpdateEmployeeAsync(int employeeId, UpdateEmployeeDto request);

        // Update own profile — limited to PhoneNumber only (all roles)
        Task<bool> UpdateMyProfileAsync(int employeeId, UpdateMyProfileDto request);

        // Soft delete an employee (Admin only)
        Task<bool> DeleteEmployeeAsync(int employeeId, int deletedByUserId);
    }
}
