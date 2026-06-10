using WMS.Application.Common;
using WMS.Application.DTOs.Leave;

namespace WMS.Application.Services
{
    /// <summary>
    /// Service interface for Leave business logic.
    /// </summary>
    public interface ILeaveService
    {
        // Employee applies for leave
        Task<(bool Success, string Message, LeaveRecordDto? Data)> ApplyLeaveAsync(int empId, ApplyLeaveDto request);

        // Employee cancels their own pending leave
        Task<(bool Success, string Message)> CancelLeaveAsync(int leaveId, int empId);

        // Get paginated leaves for the current employee
        Task<PagedResponseDto<LeaveRecordDto>> GetMyLeavesAsync(int empId, LeaveFilterDto filter);

        // Get paginated leaves for a manager's team (department)
        Task<PagedResponseDto<LeaveRecordDto>> GetTeamLeavesAsync(int managerDeptId, LeaveFilterDto filter);

        // Get all paginated leaves (Admin)
        Task<PagedResponseDto<LeaveRecordDto>> GetAllLeavesAsync(LeaveFilterDto filter);

        // Manager or Admin approves/rejects a leave
        Task<(bool Success, string Message, LeaveRecordDto? Data)> ApproveOrRejectLeaveAsync(
            int leaveId, UpdateLeaveStatusDto request, int processedByUserId, string userRole);
    }
}
