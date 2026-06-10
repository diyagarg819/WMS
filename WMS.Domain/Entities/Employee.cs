using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WMS.Domain.Entities
{
    /// <summary>
    /// Represents an employee in the workforce management system.
    /// Links to Department and Role for organizational structure.
    /// </summary>
    public class Employee
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int EmployeeId { get; set; }

        [Required]
        [MaxLength(50)]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        [MaxLength(50)]
        public string LastName { get; set; } = string.Empty;

        [Required]
        [MaxLength(80)]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        [MaxLength(15)]
        public string PhoneNumber { get; set; } = string.Empty;

        // Gender: M = Male, F = Female, O = Other
        [MaxLength(1)]
        public string? Gender { get; set; }

        // Date of birth — must be at least 18 years old (validated in service layer)
        [Required]
        public DateTime DOB { get; set; }

        // Date of joining the organization
        [Required]
        public DateTime DOJ { get; set; }

        // Foreign key to the Department this employee belongs to
        public int? DepartmentId { get; set; }

        [ForeignKey("DepartmentId")]
        public Department? Department { get; set; }

        // Foreign key to the employee's role (Admin, Manager, Employee)
        public int? RoleId { get; set; }

        [ForeignKey("RoleId")]
        public Role? Role { get; set; }

        [MaxLength(20)]
        public string Status { get; set; } = "Active";

        public DateTime CreatedOn { get; set; } = DateTime.Now;

        public DateTime? UpdatedOn { get; set; }

        // Navigation properties for related records
        public ICollection<Attendance> Attendances { get; set; } = new List<Attendance>();
        public ICollection<Leave> Leaves { get; set; } = new List<Leave>();
        public ICollection<EmployeeProjectAllocation> ProjectAllocations { get; set; } = new List<EmployeeProjectAllocation>();
    }
}
