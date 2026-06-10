namespace WMS.Application.DTOs.Attendance
{
    /// <summary>
    /// DTO for attendance list/history views.
    /// </summary>
    public class AttendanceRecordDto
    {
        public int AttendanceId { get; set; }
        public int EmpId { get; set; }
        public string? EmployeeName { get; set; }
        public DateTime CheckIn { get; set; }
        public DateTime? CheckOut { get; set; }
        public double? TotalHours { get; set; }
        public string? WorkMode { get; set; }
        public DateTime AttendanceDate { get; set; }
    }
}
