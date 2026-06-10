using WMS.Domain.Entities;

namespace WMS.Domain.Interfaces
{
    /// <summary>
    /// Repository interface for Attendance database operations.
    /// Supports check-in/out, paginated history, and duplicate-day prevention.
    /// </summary>
    public interface IAttendanceRepository
    {
        // Get paginated attendance records for a specific employee
        Task<(List<Attendance> Records, int TotalCount)> GetByEmployeeAsync(
            int empId, int pageNumber, int pageSize, DateTime? fromDate, DateTime? toDate);

        // Get paginated attendance records across all employees (Admin view)
        Task<(List<Attendance> Records, int TotalCount)> GetAllAsync(
            int pageNumber, int pageSize, DateTime? fromDate, DateTime? toDate, string? searchTerm);

        // Get a single attendance record by ID
        Task<Attendance?> GetByIdAsync(int attendanceId);

        // Check if the employee already has a record for today (prevent duplicate check-in)
        Task<Attendance?> GetTodayRecordAsync(int empId);

        // Add a new check-in record
        Task<Attendance> AddAsync(Attendance attendance);

        // Update an existing record (e.g., check-out)
        Task UpdateAsync(Attendance attendance);
    }
}
