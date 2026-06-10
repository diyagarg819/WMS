using System.ComponentModel.DataAnnotations;

namespace WMS.Application.DTOs.Attendance
{
    /// <summary>
    /// Request body for check-in. WorkMode is optional (defaults can be set by the frontend).
    /// </summary>
    public class CheckInDto
    {
        [MaxLength(20)]
        public string? WorkMode { get; set; }
    }
}
