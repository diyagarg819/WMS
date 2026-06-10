using WMS.Domain.Entities;

namespace WMS.Domain.Interfaces
{
    /// <summary>
    /// Repository interface for Leave database operations.
    /// Supports paginated retrieval (admin, team, employee levels) and audit-logged updates.
    /// </summary>
    public interface ILeaveRepository
    {
        // Get paginated leaves across all employees (Admin view)
        Task<(List<Leave> Leaves, int TotalCount)> GetAllAsync(
            int pageNumber, int pageSize, string? status, string? searchTerm);

        // Get paginated leaves for a specific department (Manager view)
        Task<(List<Leave> Leaves, int TotalCount)> GetByDepartmentAsync(
            int departmentId, int pageNumber, int pageSize, string? status, string? searchTerm);

        // Get paginated leaves for a specific employee (Employee view)
        Task<(List<Leave> Leaves, int TotalCount)> GetByEmployeeAsync(
            int empId, int pageNumber, int pageSize, string? status);

        // Get a single leave request by ID
        Task<Leave?> GetByIdAsync(int leaveId);

        // Add a new leave request (Employee applies)
        Task<Leave> AddAsync(Leave leave);

        // Update leave status (Approve/Reject) and insert an AuditLog in a single transaction
        Task UpdateStatusWithAuditAsync(Leave leave, int updatedByUserId, string actionType);

        // Delete a pending leave request (Employee cancels)
        Task DeleteAsync(Leave leave);
    }
}
