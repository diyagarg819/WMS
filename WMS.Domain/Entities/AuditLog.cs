using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WMS.Domain.Entities
{
    /// <summary>
    /// Records audit trail for specific operations only:
    /// - Employee Create, Update, Delete
    /// - Leave Approval or Rejection
    /// - Role Change
    /// - Project Allocation (assign or remove)
    /// Does NOT audit GET requests or token events.
    /// </summary>
    public class AuditLog
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int AuditId { get; set; }

        // Name of the table that was changed (e.g., "Employee", "Leave")
        [Required]
        public string EntityName { get; set; } = string.Empty;

        // Primary key of the row that was affected
        [Required]
        public int RecordId { get; set; }

        // The type of operation: Insert, Update, or Delete
        [Required]
        [MaxLength(20)]
        public string Action { get; set; } = string.Empty;

        // The UserId of the person who performed the action
        [Required]
        public int CreatedBY { get; set; }

        // When the audit record was created
        public DateTime CreatedOn { get; set; } = DateTime.Now;
    }
}
