using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WMS.Domain.Entities
{
    /// <summary>
    /// Tracks daily attendance for an employee — check-in, check-out, and total hours worked.
    /// TotalHours is a computed column configured in DbContext (not set manually).
    /// </summary>
    public class Attendance
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int AttendanceId { get; set; }

        // Foreign key to the employee who checked in
        [Required]
        public int EmpId { get; set; }

        [ForeignKey("EmpId")]
        public Employee? Employee { get; set; }

        [Required]
        public DateTime CheckIn { get; set; }

        public DateTime? CheckOut { get; set; }

        // Computed column: difference between CheckOut and CheckIn in hours
        // Configured via HasComputedColumnSql in OnModelCreating — do not set this manually
        public double? TotalHours { get; set; }

        // Work mode for the day: WFO, WFH, or Hybrid
        [MaxLength(20)]
        public string? WorkMode { get; set; }

        [Required]
        public DateTime AttendanceDate { get; set; }
    }
}
