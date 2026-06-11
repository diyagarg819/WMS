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
        [WMS.Application.Common.FutureDate(ErrorMessage = "Cannot apply for leaves in the past.")]
        public DateTime FromDate { get; set; }

        [Required(ErrorMessage = "ToDate is required.")]
        [WMS.Application.Common.DateGreaterThan("FromDate", ErrorMessage = "ToDate cannot be earlier than FromDate.")]
        public DateTime ToDate { get; set; }
    }
}
