using WMS.Application.Common;
using WMS.Application.DTOs.Department;

namespace WMS.Application.Services
{
    public interface IDepartmentService
    {
        Task<List<DepartmentDto>> GetAllAsync(SearchRequestDto request);
        Task<(bool Success, string Message, DepartmentDto? Data)> GetByIdAsync(int id);
        Task<(bool Success, string Message, DepartmentDto? Data)> CreateAsync(CreateDepartmentDto request);
        Task<(bool Success, string Message, DepartmentDto? Data)> UpdateAsync(int id, UpdateDepartmentDto request);
        Task<(bool Success, string Message)> DeleteAsync(int id);
    }
}
