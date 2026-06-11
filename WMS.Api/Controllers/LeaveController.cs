using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using WMS.Application.Common;
using WMS.Application.DTOs.Leave;
using WMS.Application.Services;

namespace WMS.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class LeaveController : ControllerBase
    {
        private readonly ILeaveService _leaveService;

        public LeaveController(ILeaveService leaveService)
        {
            _leaveService = leaveService;
        }

        private int GetCurrentUserId()
        {
            return int.TryParse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value, out int id) ? id : 0;
        }

        private string GetCurrentUserRole()
        {
            return User.FindFirst(ClaimTypes.Role)?.Value ?? string.Empty;
        }

        [HttpPost("apply")]
        public async Task<IActionResult> Apply([FromBody] ApplyLeaveDto request)
        {
            int empId = GetCurrentUserId();
            var result = await _leaveService.ApplyLeaveAsync(empId, request);

            if (!result.Success)
                return BadRequest(new ApiResponse<object?>(false, result.Message));

            return Ok(new ApiResponse<LeaveRecordDto>(true, result.Message, result.Data!));
        }

        [HttpPost("cancel/{id}")]
        public async Task<IActionResult> Cancel(int id)
        {
            int empId = GetCurrentUserId();
            var result = await _leaveService.CancelLeaveAsync(id, empId);

            if (!result.Success)
                return BadRequest(new ApiResponse<object?>(false, result.Message));

            return Ok(new ApiResponse<object?>(true, result.Message));
        }

        [HttpGet("my-history")]
        public async Task<IActionResult> GetMyHistory([FromQuery] LeaveFilterDto filter)
        {
            int empId = GetCurrentUserId();
            var result = await _leaveService.GetMyLeavesAsync(empId, filter);
            return Ok(new ApiResponse<List<LeaveRecordDto>>(true, "My leave history retrieved successfully.", result));
        }

        [HttpGet("team")]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> GetTeamLeaves([FromQuery] LeaveFilterDto filter)
        {
            int managerId = GetCurrentUserId();
            var result = await _leaveService.GetTeamLeavesForManagerAsync(managerId, filter);

            if (!result.Success)
                return BadRequest(new ApiResponse<object?>(false, result.Message));

            return Ok(new ApiResponse<List<LeaveRecordDto>>(true, "Team leave requests retrieved successfully.", result.Data!));
        }

        [HttpGet("all")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAllLeaves([FromQuery] LeaveFilterDto filter)
        {
            var result = await _leaveService.GetAllLeavesAsync(filter);
            return Ok(new ApiResponse<List<LeaveRecordDto>>(true, "All leave requests retrieved successfully.", result));
        }

        [HttpPost("approve-reject/{id}")]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> ApproveReject(int id, [FromBody] UpdateLeaveStatusDto request)
        {
            int empId = GetCurrentUserId();
            string role = GetCurrentUserRole();

            var result = await _leaveService.ApproveOrRejectLeaveAsync(id, request, empId, role);

            if (!result.Success)
                return BadRequest(new ApiResponse<object?>(false, result.Message));

            return Ok(new ApiResponse<LeaveRecordDto>(true, result.Message, result.Data!));
        }
    }
}
