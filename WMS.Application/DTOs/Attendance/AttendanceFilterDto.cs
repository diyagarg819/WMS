namespace WMS.Application.DTOs.Attendance
{
    /// <summary>
    /// Filter parameters for attendance history queries — extends the base paged request.
    /// </summary>
    public class AttendanceFilterDto
    {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public string? SearchTerm { get; set; }
    }
}
