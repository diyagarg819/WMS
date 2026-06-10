using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WMS.Domain.Entities
{
    /// <summary>
    /// Represents an organizational department (e.g., Engineering, HR, Finance).
    /// Employees are assigned to departments.
    /// </summary>
    public class Department
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int DepartmentId { get; set; }

        [Required]
        [MaxLength(100)]
        public string DepartmentName { get; set; } = string.Empty;

        [MaxLength(255)]
        public string? Description { get; set; }

        public DateTime CreatedOn { get; set; } = DateTime.Now;

        // All employees belonging to this department
        public ICollection<Employee> Employees { get; set; } = new List<Employee>();
    }
}
