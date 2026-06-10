using WMS.Domain.Entities;

namespace WMS.Domain.Interfaces
{
    /// <summary>
    /// Repository interface for Employee database operations.
    /// Supports CRUD, paginated search, and department-scoped queries.
    /// </summary>
    public interface IEmployeeRepository
    {
        // Get a paginated and filtered list of employees
        Task<(List<Employee> Employees, int TotalCount)> GetAllAsync(
            int pageNumber, int pageSize, string? searchTerm, string? sortBy, string? sortDirection);

        // Get a single employee by ID — includes Department and Role
        Task<Employee?> GetByIdAsync(int employeeId);

        // Check if an email is already in use (optionally exclude a specific employee for updates)
        Task<bool> EmailExistsAsync(string email, int? excludeEmployeeId = null);

        // Add a new employee
        Task<Employee> AddAsync(Employee employee, int createdByUserId);
        
        // Update an existing employee
        Task UpdateAsync(Employee employee, int updatedByUserId);

        // Soft delete — set Status to 'Inactive' and write an audit log entry
        Task DeleteAsync(Employee employee, int deletedByUserId);

        // Get employees filtered by department — used for Manager team-scoped queries
        Task<(List<Employee> Employees, int TotalCount)> GetByDepartmentAsync(
            int departmentId, int pageNumber, int pageSize, string? searchTerm);
    }
}
