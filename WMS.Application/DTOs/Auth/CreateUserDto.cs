using System.ComponentModel.DataAnnotations;

namespace WMS.Application.DTOs.Auth
{
    /// <summary>
    /// Request body for creating a new user login — Admin only.
    /// The admin links an existing employee record to login credentials.
    /// </summary>
    public class CreateUserDto
    {
        [Required(ErrorMessage = "Employee ID is required")]
        public int EmployeeId { get; set; }

        [Required(ErrorMessage = "Username is required")]
        [MaxLength(50)]
        public string Username { get; set; } = string.Empty;

        [Required(ErrorMessage = "Password is required")]
        [MinLength(8, ErrorMessage = "Password must be at least 8 characters")]
        public string Password { get; set; } = string.Empty;

        [Required(ErrorMessage = "Role ID is required")]
        public int RoleId { get; set; }
    }
}
