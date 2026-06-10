using WMS.Domain.Entities;

namespace WMS.Domain.Interfaces
{
    public interface IDepartmentRepository
    {
        Task<(List<Department> Departments, int TotalCount)> GetAllAsync(int pageNumber, int pageSize, string? searchTerm);
        Task<Department?> GetByIdAsync(int id);
        Task<Department?> GetByNameAsync(string name);
        Task<Department> AddAsync(Department department);
        Task UpdateAsync(Department department);
        Task DeleteAsync(Department department);
    }
}
