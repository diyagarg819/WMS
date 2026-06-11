using WMS.Application.DTOs.Attendance;

namespace WMS.Application.Services
{
    public interface IAttendanceService
    {
        Task<AttendanceRecordDto?> CheckInAsync(int empId, CheckInDto request);
        Task<AttendanceRecordDto?> CheckOutAsync(int empId);
        Task<AttendanceRecordDto?> GetTodayStatusAsync(int empId);
        Task<List<AttendanceRecordDto>> GetMyAttendanceAsync(int empId, AttendanceFilterDto filter);
        Task<List<AttendanceRecordDto>> GetAllAttendanceAsync(AttendanceFilterDto filter);
    }
}
