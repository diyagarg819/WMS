using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WMS.Application.Common;
using WMS.Application.DTOs.Attendance;
using WMS.Application.Services;

namespace WMS.Api.Controllers
{
    /// <summary>
    /// Attendance management — check-in/out, today's status, and history.
    /// Employees see their own data. Admin/Manager see all.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class AttendanceController : ControllerBase
    {
        private readonly IAttendanceService _attendanceService;
        private readonly ILogger<AttendanceController> _logger;

        public AttendanceController(IAttendanceService attendanceService, ILogger<AttendanceController> logger)
        {
            _attendanceService = attendanceService;
            _logger = logger;
        }

        // POST /api/attendance/check-in — employee checks in for the day
        [HttpPost("check-in")]
        public async Task<IActionResult> CheckIn([FromBody] CheckInDto request)
        {
            int empId = GetUserIdFromToken();

            var result = await _attendanceService.CheckInAsync(empId, request);

            if (result == null)
                return Conflict(new ApiResponse<object>(false, "Already checked in today"));

            return StatusCode(201, new ApiResponse<AttendanceRecordDto>(true, "Checked in successfully", result));
        }

        // POST /api/attendance/check-out — employee checks out for the day
        [HttpPost("check-out")]
        public async Task<IActionResult> CheckOut()
        {
            int empId = GetUserIdFromToken();

            var result = await _attendanceService.CheckOutAsync(empId);

            if (result == null)
                return Conflict(new ApiResponse<object>(false, "Not checked in today or already checked out"));

            return Ok(new ApiResponse<AttendanceRecordDto>(true, "Checked out successfully", result));
        }

        // GET /api/attendance/today — get today's attendance status for the current user
        [HttpGet("today")]
        public async Task<IActionResult> GetTodayStatus()
        {
            int empId = GetUserIdFromToken();

            var result = await _attendanceService.GetTodayStatusAsync(empId);

            if (result == null)
                return Ok(new ApiResponse<object?>(true, "Not checked in today", null));

            return Ok(new ApiResponse<AttendanceRecordDto>(true, "Today's attendance", result));
        }

        // GET /api/attendance/my-history — employee's own attendance history
        [HttpGet("my-history")]
        public async Task<IActionResult> GetMyHistory([FromQuery] AttendanceFilterDto filter)
        {
            int empId = GetUserIdFromToken();

            var result = await _attendanceService.GetMyAttendanceAsync(empId, filter);

            return Ok(new ApiResponse<PagedResponseDto<AttendanceRecordDto>>(true, "Attendance history", result));
        }

        // GET /api/attendance — Admin/Manager view of all attendance records
        [Authorize(Roles = "Admin,Manager")]
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] AttendanceFilterDto filter)
        {
            var result = await _attendanceService.GetAllAttendanceAsync(filter);

            return Ok(new ApiResponse<PagedResponseDto<AttendanceRecordDto>>(true, "All attendance records", result));
        }

        private int GetUserIdFromToken()
        {
            var claim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return int.TryParse(claim, out int id) ? id : 0;
        }
    }
}
