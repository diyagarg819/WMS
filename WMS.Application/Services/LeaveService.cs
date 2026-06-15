using Microsoft.Extensions.Logging;
using WMS.Application.Common;
using WMS.Application.DTOs.Leave;
using WMS.Domain.Entities;
using WMS.Domain.Interfaces;

namespace WMS.Application.Services
{
    /// <summary>
    /// Business logic for the Leave module.
    /// Handles applying, cancelling, approving/rejecting, and RBAC-scoped data queries.
    /// </summary>
    public class LeaveService : ILeaveService
    {
        private readonly ILeaveRepository _leaveRepository;
        private readonly IEmployeeRepository _employeeRepository;
        private readonly ILogger<LeaveService> _logger;

        public LeaveService(
            ILeaveRepository leaveRepository,
            IEmployeeRepository employeeRepository,
            ILogger<LeaveService> logger)
        {
            _leaveRepository = leaveRepository;
            _employeeRepository = employeeRepository;
            _logger = logger;
        }

        public async Task<(bool Success, string Message, LeaveRecordDto? Data)> ApplyLeaveAsync(int empId, ApplyLeaveDto request)
        {
            // Date validations are now handled by [FutureDate] and [DateGreaterThan] attributes

            // Ensure employee exists
            var employee = await _employeeRepository.GetByIdAsync(empId);
            if (employee == null)
            {
                return (false, "Employee not found.", null);
            }

            var leave = new Leave
            {
                EmpId = empId,
                LeaveType = request.LeaveType,
                Reason = request.Reason,
                FromDate = request.FromDate.Date,
                ToDate = request.ToDate.Date,
                Status = "Pending",
                AppliedOn = DateTime.Now
            };

            var created = await _leaveRepository.AddAsync(leave);

            // Fetch again to include employee navigation property for mapping
            var updated = await _leaveRepository.GetByIdAsync(created.LeaveId);

            _logger.LogInformation("Leave request {LeaveId} applied by employee {EmpId}.", created.LeaveId, empId);

            return (true, "Leave applied successfully.", MapToDto(updated!));
        }

        public async Task<(bool Success, string Message)> CancelLeaveAsync(int leaveId, int empId)
        {
            var leave = await _leaveRepository.GetByIdAsync(leaveId);

            if (leave == null)
            {
                return (false, "Leave request not found.");
            }

            if (leave.EmpId != empId)
            {
                return (false, "You can only cancel your own leave requests.");
            }

            if (leave.Status != "Pending")
            {
                return (false, "You cannot cancel a leave request that has already been processed.");
            }

            await _leaveRepository.DeleteAsync(leave);

            _logger.LogInformation("Pending leave {LeaveId} was cancelled by employee {EmpId}.", leaveId, empId);

            return (true, "Leave cancelled successfully.");
        }

        public async Task<List<LeaveRecordDto>> GetMyLeavesAsync(int empId, LeaveFilterDto filter)
        {
            var records = await _leaveRepository.GetByEmployeeAsync(
                empId, filter.Status, filter.SearchTerm);

            return records.Select(MapToDto).ToList();
        }

        public async Task<(bool Success, string Message, List<LeaveRecordDto>? Data)> GetTeamLeavesForManagerAsync(int managerId, LeaveFilterDto filter)
        {
            var manager = await _employeeRepository.GetByIdAsync(managerId);
            if (manager == null)
                return (false, "Manager profile not found.", null);

            if (manager.DepartmentId == null)
                return (false, "Manager is not assigned to any department.", null);

            var records = await _leaveRepository.GetByDepartmentAsync(
                manager.DepartmentId.Value, filter.Status, filter.SearchTerm);

            var result = records.Select(MapToDto).ToList();

            return (true, "Team leave requests retrieved successfully.", result);
        }

        public async Task<List<LeaveRecordDto>> GetAllLeavesAsync(LeaveFilterDto filter)
        {
            var records = await _leaveRepository.GetAllAsync(
                filter.Status, filter.SearchTerm);

            return records.Select(MapToDto).ToList();
        }

        public async Task<(bool Success, string Message, LeaveRecordDto? Data)> ApproveOrRejectLeaveAsync(
            int leaveId, UpdateLeaveStatusDto request, int processedByUserId, string userRole)
        {
            var leave = await _leaveRepository.GetByIdAsync(leaveId);
            if (leave == null)
            {
                return (false, "Leave request not found.", null);
            }

            // Ensure valid status string is now handled by [AllowedValues] attribute

            // Business Rule: Admin can override anything. Manager can only process 'Pending' leaves.
            if (leave.Status != "Pending" && userRole != "Admin")
            {
                return (false, "This leave has already been processed and only an Admin can override it.", null);
            }

            // For Managers, ensure the leave belongs to someone in their department
            if (userRole == "Manager")
            {
                var managerUser = await _employeeRepository.GetByIdAsync(processedByUserId);
                if (managerUser == null || managerUser.DepartmentId != leave.Employee!.DepartmentId)
                {
                    return (false, "You can only process leaves for employees in your department.", null);
                }
            }

            leave.Status = request.Status;
            leave.ApprovedBy = processedByUserId;
            leave.ApprovedOn = DateTime.Now;

            string actionType = request.Status == "Approved" ? "Leave Approval" : "Leave Rejection";

            // Save and insert AuditLog via repository transaction
            await _leaveRepository.UpdateStatusWithAuditAsync(leave, processedByUserId, actionType);

            _logger.LogInformation("Leave {LeaveId} was marked as {Status} by user {UserId}.", leaveId, request.Status, processedByUserId);

            return (true, $"Leave successfully {request.Status.ToLower()}.", MapToDto(leave));
        }

        private LeaveRecordDto MapToDto(Leave leave)
        {
            return new LeaveRecordDto
            {
                LeaveId = leave.LeaveId,
                EmpId = leave.EmpId,
                EmployeeName = leave.Employee != null ? $"{leave.Employee.FirstName} {leave.Employee.LastName}" : null,
                LeaveType = leave.LeaveType,
                Reason = leave.Reason,
                FromDate = leave.FromDate,
                ToDate = leave.ToDate,
                Status = leave.Status,
                AppliedOn = leave.AppliedOn,
                ApprovedBy = leave.ApprovedBy,
                ApprovedOn = leave.ApprovedOn
            };
        }
    }
}
