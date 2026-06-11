using WMS.Domain.Entities;

namespace WMS.Domain.Interfaces
{
    public interface IEmployeeRepository
    {
        Task<List<Employee>> GetAllAsync(string? searchTerm);
        Task<Employee?> GetByIdAsync(int employeeId);
        Task<bool> EmailExistsAsync(string email, int? excludeEmployeeId = null);
        Task<Employee> AddAsync(Employee employee, int createdByUserId);
        Task UpdateAsync(Employee employee, int updatedByUserId);
        Task DeleteAsync(Employee employee, int deletedByUserId);
        Task<List<Employee>> GetByDepartmentAsync(int departmentId, string? searchTerm);
    }
}
