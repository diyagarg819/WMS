using WMS.Application.Common;
using WMS.Application.DTOs.Attendance;

namespace WMS.Application.Services
{
    /// <summary>
    /// Service interface for Attendance operations.
    /// </summary>
    public interface IAttendanceService
    {
        // Employee checks in for the day — only one check-in per day allowed
        Task<AttendanceRecordDto?> CheckInAsync(int empId, CheckInDto request);

        // Employee checks out — updates the existing record for today
        Task<AttendanceRecordDto?> CheckOutAsync(int empId);

        // Get the current day's attendance status for an employee
        Task<AttendanceRecordDto?> GetTodayStatusAsync(int empId);

        // Get paginated attendance history for a specific employee
        Task<PagedResponseDto<AttendanceRecordDto>> GetMyAttendanceAsync(int empId, AttendanceFilterDto filter);

        // Get paginated attendance records across all employees (Admin view)
        Task<PagedResponseDto<AttendanceRecordDto>> GetAllAttendanceAsync(AttendanceFilterDto filter);
    }
}
