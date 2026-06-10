namespace WMS.Application.DTOs.Employee
{
    /// <summary>
    /// Lightweight DTO for employee list views — only the fields needed for the table.
    /// </summary>
    public class EmployeeListDto
    {
        public int EmployeeId { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string? DepartmentName { get; set; }
        public string? RoleName { get; set; }
        public string Status { get; set; } = string.Empty;
    }
}
