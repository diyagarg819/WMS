using System.ComponentModel.DataAnnotations;

namespace WMS.Application.DTOs.Leave
{
    /// <summary>
    /// DTO for applying for a new leave request.
    /// Includes necessary validation rules via Data Annotations.
    /// </summary>
    public class ApplyLeaveDto
    {
        [Required(ErrorMessage = "LeaveType is required.")]
        [MaxLength(30)]
        public string LeaveType { get; set; } = string.Empty;

        [MaxLength(255)]
        public string? Reason { get; set; }

        [Required(ErrorMessage = "FromDate is required.")]
        public DateTime FromDate { get; set; }

        [Required(ErrorMessage = "ToDate is required.")]
        public DateTime ToDate { get; set; }
    }
}
