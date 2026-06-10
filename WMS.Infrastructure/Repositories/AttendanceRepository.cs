using Microsoft.EntityFrameworkCore;
using WMS.Domain.Entities;
using WMS.Domain.Interfaces;
using WMS.Infrastructure.Data;

namespace WMS.Infrastructure.Repositories
{
    /// <summary>
    /// Repository for Attendance database operations.
    /// </summary>
    public class AttendanceRepository : IAttendanceRepository
    {
        private readonly WMSDbContext _context;

        public AttendanceRepository(WMSDbContext context)
        {
            _context = context;
        }

        // Get paginated attendance records for a specific employee with optional date range
        public async Task<(List<Attendance> Records, int TotalCount)> GetByEmployeeAsync(
            int empId, int pageNumber, int pageSize, DateTime? fromDate, DateTime? toDate)
        {
            var query = _context.Attendances
                .Include(a => a.Employee)
                .Where(a => a.EmpId == empId);

            // Apply date range filter
            if (fromDate.HasValue)
                query = query.Where(a => a.AttendanceDate >= fromDate.Value.Date);

            if (toDate.HasValue)
                query = query.Where(a => a.AttendanceDate <= toDate.Value.Date);

            int totalCount = await query.CountAsync();

            var records = await query
                .OrderByDescending(a => a.AttendanceDate)
                .ThenByDescending(a => a.CheckIn)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (records, totalCount);
        }

        // Get paginated attendance records across all employees (Admin view)
        public async Task<(List<Attendance> Records, int TotalCount)> GetAllAsync(
            int pageNumber, int pageSize, DateTime? fromDate, DateTime? toDate, string? searchTerm)
        {
            var query = _context.Attendances
                .Include(a => a.Employee)
                .AsQueryable();

            if (fromDate.HasValue)
                query = query.Where(a => a.AttendanceDate >= fromDate.Value.Date);

            if (toDate.HasValue)
                query = query.Where(a => a.AttendanceDate <= toDate.Value.Date);

            // Search by employee name
            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                string term = searchTerm.Trim().ToLower();
                query = query.Where(a =>
                    a.Employee!.FirstName.ToLower().Contains(term) ||
                    a.Employee!.LastName.ToLower().Contains(term));
            }

            int totalCount = await query.CountAsync();

            var records = await query
                .OrderByDescending(a => a.AttendanceDate)
                .ThenByDescending(a => a.CheckIn)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (records, totalCount);
        }

        // Get a single attendance record by ID
        public async Task<Attendance?> GetByIdAsync(int attendanceId)
        {
            return await _context.Attendances
                .Include(a => a.Employee)
                .FirstOrDefaultAsync(a => a.AttendanceId == attendanceId);
        }

        // Check if the employee already has a record for today
        public async Task<Attendance?> GetTodayRecordAsync(int empId)
        {
            return await _context.Attendances
                .Include(a => a.Employee)
                .FirstOrDefaultAsync(a => a.EmpId == empId && a.AttendanceDate == DateTime.Today);
        }

        // Add a new attendance record
        public async Task<Attendance> AddAsync(Attendance attendance)
        {
            await _context.Attendances.AddAsync(attendance);
            await _context.SaveChangesAsync();
            return attendance;
        }

        // Update an existing attendance record
        public async Task UpdateAsync(Attendance attendance)
        {
            _context.Attendances.Update(attendance);
            await _context.SaveChangesAsync();
        }
    }
}
