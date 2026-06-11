using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WMS.Application.Common;
using WMS.Application.DTOs.Attendance;
using WMS.Application.Services;

namespace WMS.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class AttendanceController : ControllerBase
    {
        private readonly IAttendanceService _attendanceService;

        public AttendanceController(IAttendanceService attendanceService)
        {
            _attendanceService = attendanceService;
        }

        [HttpPost("check-in")]
        public async Task<IActionResult> CheckIn([FromBody] CheckInDto request)
        {
            int empId = GetUserIdFromToken();
            var result = await _attendanceService.CheckInAsync(empId, request);

            if (result == null)
                return Conflict(new ApiResponse<object>(false, "Already checked in today"));

            return StatusCode(201, new ApiResponse<AttendanceRecordDto>(true, "Checked in successfully", result));
        }

        [HttpPost("check-out")]
        public async Task<IActionResult> CheckOut()
        {
            int empId = GetUserIdFromToken();
            var result = await _attendanceService.CheckOutAsync(empId);

            if (result == null)
                return Conflict(new ApiResponse<object>(false, "Not checked in today or already checked out"));

            return Ok(new ApiResponse<AttendanceRecordDto>(true, "Checked out successfully", result));
        }

        [HttpGet("today")]
        public async Task<IActionResult> GetTodayStatus()
        {
            int empId = GetUserIdFromToken();
            var result = await _attendanceService.GetTodayStatusAsync(empId);

            if (result == null)
                return Ok(new ApiResponse<object?>(true, "Not checked in today"));

            return Ok(new ApiResponse<AttendanceRecordDto>(true, "Today's attendance", result));
        }

        [HttpGet("my-history")]
        public async Task<IActionResult> GetMyHistory([FromQuery] AttendanceFilterDto filter)
        {
            int empId = GetUserIdFromToken();
            var result = await _attendanceService.GetMyAttendanceAsync(empId, filter);
            return Ok(new ApiResponse<List<AttendanceRecordDto>>(true, "Attendance history", result));
        }

        [Authorize(Roles = "Admin,Manager")]
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] AttendanceFilterDto filter)
        {
            var result = await _attendanceService.GetAllAttendanceAsync(filter);
            return Ok(new ApiResponse<List<AttendanceRecordDto>>(true, "All attendance records", result));
        }

        private int GetUserIdFromToken()
        {
            var claim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return int.TryParse(claim, out int id) ? id : 0;
        }
    }
}
