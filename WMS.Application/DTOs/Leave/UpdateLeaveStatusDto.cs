using System.ComponentModel.DataAnnotations;

namespace WMS.Application.DTOs.Leave
{
    /// <summary>
    /// DTO for updating the status of a leave request (Approve/Reject/Override).
    /// </summary>
    public class UpdateLeaveStatusDto
    {
        [Required(ErrorMessage = "Status is required.")]
        [MaxLength(20)]
        [WMS.Application.Common.AllowedValues("Approved", "Rejected", ErrorMessage = "Status must be either 'Approved' or 'Rejected'.")]
        public string Status { get; set; } = string.Empty;
    }
}
