using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WMS.Domain.Entities
{
    /// <summary>
    /// Represents a project that employees can be allocated to.
    /// Optionally linked to a Client.
    /// </summary>
    public class Project
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ProjectId { get; set; }

        [Required]
        [MaxLength(100)]
        public string ProjectName { get; set; } = string.Empty;

        // Optional link to the client who owns this project
        public int? ClientId { get; set; }

        [ForeignKey("ClientId")]
        public Client? Client { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        [MaxLength(20)]
        public string Status { get; set; } = "Active";

        // Employees allocated to this project
        public ICollection<EmployeeProjectAllocation> EmployeeAllocations { get; set; } = new List<EmployeeProjectAllocation>();
    }
}
