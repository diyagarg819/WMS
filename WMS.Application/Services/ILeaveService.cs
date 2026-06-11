using WMS.Application.DTOs.Leave;

namespace WMS.Application.Services
{
    public interface ILeaveService
    {
        Task<(bool Success, string Message, LeaveRecordDto? Data)> ApplyLeaveAsync(int empId, ApplyLeaveDto request);
        Task<(bool Success, string Message)> CancelLeaveAsync(int leaveId, int empId);
        Task<List<LeaveRecordDto>> GetMyLeavesAsync(int empId, LeaveFilterDto filter);
        Task<(bool Success, string Message, List<LeaveRecordDto>? Data)> GetTeamLeavesForManagerAsync(int managerId, LeaveFilterDto filter);
        Task<List<LeaveRecordDto>> GetAllLeavesAsync(LeaveFilterDto filter);
        Task<(bool Success, string Message, LeaveRecordDto? Data)> ApproveOrRejectLeaveAsync(
            int leaveId, UpdateLeaveStatusDto request, int processedByUserId, string userRole);
    }
}
