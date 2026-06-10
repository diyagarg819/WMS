using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WMS.Domain.Entities
{
    /// <summary>
    /// Stores login credentials for employees.
    /// Created by Admin only — there is no public registration in this system.
    /// Includes RefreshToken and RefreshTokenExpiry for the access/refresh token auth flow.
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

        // Current refresh token — rotated on every token refresh call
        public string? RefreshToken { get; set; }

        // When the refresh token expires — set to null on logout
        public DateTime? RefreshTokenExpiry { get; set; }

        // Timestamp of the last successful login
        public DateTime? LastLogin { get; set; }
    }
}
