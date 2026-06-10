using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WMS.Domain.Entities
{
    /// <summary>
    /// Tracks which employees are assigned to which projects.
    /// Acts as a many-to-many join table between Employee and Project.
    /// </summary>
    public class EmployeeProjectAllocation
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int AllocationId { get; set; }

        // The employee being assigned to a project
        [Required]
        public int EmpId { get; set; }

        [ForeignKey("EmpId")]
        public Employee? Employee { get; set; }

        // The project the employee is assigned to
        [Required]
        public int ProjectId { get; set; }

        [ForeignKey("ProjectId")]
        public Project? Project { get; set; }

        [Required]
        public DateTime AssignedOn { get; set; }

        [Required]
        public DateTime CreateDate { get; set; }

        // Name or identifier of the person who created this allocation
        [Required]
        [MaxLength(50)]
        public string CreatedBY { get; set; } = string.Empty;

        // Whether this allocation is currently active
        public bool Status { get; set; } = true;

        [MaxLength(50)]
        public string? UpdatedBy { get; set; }

        public DateTime? UpdatedDate { get; set; }
    }
}
