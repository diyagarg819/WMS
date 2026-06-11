using Microsoft.Extensions.Logging;
using WMS.Application.Common;
using WMS.Application.DTOs.Attendance;
using WMS.Domain.Entities;
using WMS.Domain.Interfaces;

namespace WMS.Application.Services
{
    /// <summary>
    /// Attendance business logic — check-in/out with duplicate prevention, history queries.
    /// TotalHours is computed by SQL Server — never calculated in code.
    /// </summary>
    public class AttendanceService : IAttendanceService
    {
        private readonly IAttendanceRepository _attendanceRepository;
        private readonly ILogger<AttendanceService> _logger;

        public AttendanceService(IAttendanceRepository attendanceRepository, ILogger<AttendanceService> logger)
        {
            _attendanceRepository = attendanceRepository;
            _logger = logger;
        }

        // Check in for today — fail if already checked in
        public async Task<AttendanceRecordDto?> CheckInAsync(int empId, CheckInDto request)
        {
            // Prevent duplicate check-in for the same day
            var existing = await _attendanceRepository.GetTodayRecordAsync(empId);
            if (existing != null)
            {
                _logger.LogWarning("Check-in failed — employee {EmpId} already checked in today", empId);
                return null;
            }

            var attendance = new Attendance
            {
                EmpId = empId,
                CheckIn = DateTime.Now,
                WorkMode = request.WorkMode,
                AttendanceDate = DateTime.Today
            };

            var created = await _attendanceRepository.AddAsync(attendance);

            _logger.LogInformation("Employee {EmpId} checked in at {Time}", empId, created.CheckIn);

            return MapToDto(created);
        }

        // Check out — updates today's record with the current time
        public async Task<AttendanceRecordDto?> CheckOutAsync(int empId)
        {
            // Find today's record — must exist and not already checked out
            var record = await _attendanceRepository.GetTodayRecordAsync(empId);
            if (record == null)
            {
                _logger.LogWarning("Check-out failed — employee {EmpId} has not checked in today", empId);
                return null;
            }

            if (record.CheckOut != null)
            {
                _logger.LogWarning("Check-out failed — employee {EmpId} already checked out today", empId);
                return null;
            }

            record.CheckOut = DateTime.Now;
            await _attendanceRepository.UpdateAsync(record);

            // Re-fetch to get the SQL-computed TotalHours value
            var updated = await _attendanceRepository.GetByIdAsync(record.AttendanceId);

            _logger.LogInformation("Employee {EmpId} checked out at {Time}", empId, record.CheckOut);

            return updated != null ? MapToDto(updated) : MapToDto(record);
        }

        // Get today's attendance status for an employee
        public async Task<AttendanceRecordDto?> GetTodayStatusAsync(int empId)
        {
            var record = await _attendanceRepository.GetTodayRecordAsync(empId);
            return record != null ? MapToDto(record) : null;
        }

        // Get attendance history for a specific employee
        public async Task<List<AttendanceRecordDto>> GetMyAttendanceAsync(
            int empId, AttendanceFilterDto filter)
        {
            var records = await _attendanceRepository.GetByEmployeeAsync(
                empId, filter.FromDate, filter.ToDate);

            return records.Select(r => MapToDto(r)).ToList();
        }

        // Get attendance for all employees (Admin view)
        public async Task<List<AttendanceRecordDto>> GetAllAttendanceAsync(AttendanceFilterDto filter)
        {
            var records = await _attendanceRepository.GetAllAsync(
                filter.FromDate, filter.ToDate, filter.SearchTerm);

            return records.Select(r => MapToDto(r)).ToList();
        }

        // Map entity to DTO
        private AttendanceRecordDto MapToDto(Attendance attendance)
        {
            return new AttendanceRecordDto
            {
                AttendanceId = attendance.AttendanceId,
                EmpId = attendance.EmpId,
                EmployeeName = attendance.Employee != null
                    ? $"{attendance.Employee.FirstName} {attendance.Employee.LastName}"
                    : null,
                CheckIn = attendance.CheckIn,
                CheckOut = attendance.CheckOut,
                TotalHours = attendance.TotalHours,
                WorkMode = attendance.WorkMode,
                AttendanceDate = attendance.AttendanceDate
            };
        }
    }
}
