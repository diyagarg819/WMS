using Microsoft.EntityFrameworkCore;
using WMS.Domain.Entities;
using WMS.Domain.Interfaces;
using WMS.Infrastructure.Data;

namespace WMS.Infrastructure.Repositories
{
    public class AttendanceRepository : IAttendanceRepository
    {
        private readonly WMSDbContext _context;

        public AttendanceRepository(WMSDbContext context)
        {
            _context = context;
        }

        public async Task<List<Attendance>> GetByEmployeeAsync(
            int empId, DateTime? fromDate, DateTime? toDate)
        {
            var query = _context.Attendances
                .Include(a => a.Employee)
                .Where(a => a.EmpId == empId);

            if (fromDate.HasValue)
                query = query.Where(a => a.AttendanceDate >= fromDate.Value.Date);

            if (toDate.HasValue)
                query = query.Where(a => a.AttendanceDate <= toDate.Value.Date);

            return await query
                .OrderByDescending(a => a.AttendanceDate)
                .ThenByDescending(a => a.CheckIn)
                .ToListAsync();
        }

        public async Task<List<Attendance>> GetAllAsync(
            DateTime? fromDate, DateTime? toDate, string? searchTerm)
        {
            var query = _context.Attendances
                .Include(a => a.Employee)
                .AsQueryable();

            if (fromDate.HasValue)
                query = query.Where(a => a.AttendanceDate >= fromDate.Value.Date);

            if (toDate.HasValue)
                query = query.Where(a => a.AttendanceDate <= toDate.Value.Date);

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                string term = searchTerm.Trim().ToLower();
                query = query.Where(a =>
                    a.Employee!.FirstName.ToLower().Contains(term) ||
                    a.Employee!.LastName.ToLower().Contains(term));
            }

            return await query
                .OrderByDescending(a => a.AttendanceDate)
                .ThenByDescending(a => a.CheckIn)
                .ToListAsync();
        }

        public async Task<Attendance?> GetByIdAsync(int attendanceId)
        {
            return await _context.Attendances
                .Include(a => a.Employee)
                .FirstOrDefaultAsync(a => a.AttendanceId == attendanceId);
        }

        public async Task<Attendance?> GetTodayRecordAsync(int empId)
        {
            return await _context.Attendances
                .Include(a => a.Employee)
                .FirstOrDefaultAsync(a => a.EmpId == empId && a.AttendanceDate == DateTime.Today);
        }

        public async Task<Attendance> AddAsync(Attendance attendance)
        {
            await _context.Attendances.AddAsync(attendance);
            await _context.SaveChangesAsync();
            return attendance;
        }

        public async Task UpdateAsync(Attendance attendance)
        {
            _context.Attendances.Update(attendance);
            await _context.SaveChangesAsync();
        }
    }
}
