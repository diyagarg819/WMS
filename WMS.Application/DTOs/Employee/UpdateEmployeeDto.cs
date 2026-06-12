using System.ComponentModel.DataAnnotations;

namespace WMS.Application.DTOs.Employee
{
    /// <summary>
    /// Request body for updating an employee — Admin can update all fields.
    /// Employee can only update their own PhoneNumber via the /my-profile endpoint.
    /// </summary>
    public class UpdateEmployeeDto
    {
        [Required]
        [MaxLength(50)]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        [MaxLength(50)]
        public string LastName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email is required")]
        [RegularExpression(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$", ErrorMessage = "Please enter a valid email address")]
        [MaxLength(80)]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Phone Number is required")]
        [RegularExpression(@"^\d{10}$", ErrorMessage = "Phone Number must be exactly 10 digits")]
        [MaxLength(15)]
        public string PhoneNumber { get; set; } = string.Empty;

        [MaxLength(1)]
        public string? Gender { get; set; }

        [Required]
        [WMS.Application.Common.MinimumAge(18)]
        public DateTime DOB { get; set; }

        [Required]
        public DateTime DOJ { get; set; }

        public int? DepartmentId { get; set; }

        public int? RoleId { get; set; }

        [MaxLength(20)]
        public string? Status { get; set; }

        /// <summary>
        /// Login username — can be changed by Admin.
        /// </summary>
        [Required(ErrorMessage = "Username is required")]
        [MaxLength(50)]
        public string Username { get; set; } = string.Empty;

        private string? _password;

        /// <summary>
        /// New password — optional. If provided, password will be updated (hashed).
        /// Leave empty to keep the current password.
        /// </summary>
        [MinLength(6, ErrorMessage = "Password must be at least 6 characters")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[!@#\$%\^&\*]).{6,}$", ErrorMessage = "Password must contain at least one uppercase letter, one lowercase letter, one number, and one special character")]
        public string? Password 
        { 
            get => _password; 
            set => _password = string.IsNullOrWhiteSpace(value) ? null : value; 
        }
    }
}
