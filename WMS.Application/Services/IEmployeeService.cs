using WMS.Application.Common;
using WMS.Application.DTOs.Employee;

namespace WMS.Application.Services
{
    public interface IEmployeeService
    {
        Task<List<EmployeeListDto>> GetAllEmployeesAsync(SearchRequestDto request);
        Task<List<EmployeeListDto>> GetEmployeesByDepartmentAsync(int departmentId, SearchRequestDto request);
        Task<EmployeeDetailDto?> GetEmployeeByIdAsync(int employeeId);
        Task<EmployeeDetailDto?> CreateEmployeeAsync(CreateEmployeeDto request, int userId);
        Task<bool> UpdateEmployeeAsync(int employeeId, UpdateEmployeeDto request, int userId);
        Task<bool> UpdateMyProfileAsync(int employeeId, UpdateMyProfileDto request, int userId);
        Task<bool> DeleteEmployeeAsync(int employeeId, int deletedByUserId);
    }
}
