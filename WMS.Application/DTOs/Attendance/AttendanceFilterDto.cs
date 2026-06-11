using WMS.Application.Common;

namespace WMS.Application.DTOs.Attendance
{
    public class AttendanceFilterDto : SearchRequestDto
    {
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
    }
}
