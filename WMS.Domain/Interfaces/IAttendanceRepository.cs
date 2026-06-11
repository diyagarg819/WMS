using WMS.Domain.Entities;

namespace WMS.Domain.Interfaces
{
    public interface IAttendanceRepository
    {
        Task<List<Attendance>> GetByEmployeeAsync(int empId, DateTime? fromDate, DateTime? toDate);
        Task<List<Attendance>> GetAllAsync(DateTime? fromDate, DateTime? toDate, string? searchTerm);
        Task<Attendance?> GetByIdAsync(int attendanceId);
        Task<Attendance?> GetTodayRecordAsync(int empId);
        Task<Attendance> AddAsync(Attendance attendance);
        Task UpdateAsync(Attendance attendance);
    }
}
