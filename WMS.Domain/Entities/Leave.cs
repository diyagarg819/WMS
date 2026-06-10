using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WMS.Domain.Entities
{
    /// <summary>
    /// Represents a leave request submitted by an employee.
    /// Follows the workflow: Employee applies → Manager approves/rejects → status updated.
    /// </summary>
    public class Leave
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int LeaveId { get; set; }

        // The employee who requested the leave
        [Required]
        public int EmpId { get; set; }

        [ForeignKey("EmpId")]
        public Employee? Employee { get; set; }

        // Type of leave: Sick, Casual, or Earned
        [Required]
        [MaxLength(30)]
        public string LeaveType { get; set; } = string.Empty;

        [MaxLength(255)]
        public string? Reason { get; set; }

        [Required]
        public DateTime FromDate { get; set; }

        [Required]
        public DateTime ToDate { get; set; }

        // Current status of the leave request: Pending, Approved, or Rejected
        [MaxLength(20)]
        public string Status { get; set; } = "Pending";

        public DateTime AppliedOn { get; set; } = DateTime.Now;

        // The manager (EmployeeId) who approved or rejected this leave
        public int? ApprovedBy { get; set; }

        public DateTime? ApprovedOn { get; set; }
    }
}
