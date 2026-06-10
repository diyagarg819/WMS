using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WMS.Domain.Entities
{
    /// <summary>
    /// Represents a user role in the system.
    /// Seed data: Admin (1), Manager (2), Employee (3).
    /// </summary>
    public class Role
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int RoleId { get; set; }

        [Required]
        [MaxLength(50)]
        public string RoleName { get; set; } = string.Empty;

        [MaxLength(150)]
        public string? Description { get; set; }

        // Employees assigned to this role
        public ICollection<Employee> Employees { get; set; } = new List<Employee>();

        // User logins associated with this role
        public ICollection<UserLogin> UserLogins { get; set; } = new List<UserLogin>();
    }
}
