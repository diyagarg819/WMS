using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WMS.Domain.Entities
{
    /// <summary>
    /// Stores login credentials for employees.
    /// Created by Admin only — there is no public registration in this system.
    /// Uses BCrypt for storing the password hash.
    /// </summary>
    public class UserLogin
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int UserId { get; set; }

        [Required]
        [MaxLength(50)]
        public string Username { get; set; } = string.Empty;

        // BCrypt-hashed password — never stored or logged in plain text
        [Required]
        public string PasswordHash { get; set; } = string.Empty;

        // The role assigned to this login (Admin, Manager, Employee)
        [Required]
        public int RoleId { get; set; }

        [ForeignKey("RoleId")]
        public Role? Role { get; set; }

        // Timestamp of the last successful login
        public DateTime? LastLogin { get; set; }
    }
}
