using WMS.Domain.Entities;

namespace WMS.Domain.Interfaces
{
    public interface ILeaveRepository
    {
        Task<List<Leave>> GetAllAsync(string? status, string? searchTerm);
        Task<List<Leave>> GetByDepartmentAsync(int departmentId, string? status, string? searchTerm);
        Task<List<Leave>> GetByEmployeeAsync(int empId, string? status);
        Task<Leave?> GetByIdAsync(int leaveId);
        Task<Leave> AddAsync(Leave leave);
        Task UpdateStatusWithAuditAsync(Leave leave, int updatedByUserId, string actionType);
        Task DeleteAsync(Leave leave);
    }
}
