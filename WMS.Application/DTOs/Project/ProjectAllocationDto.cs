namespace WMS.Application.DTOs.Project
{
    public class ProjectAllocationDto
    {
        public int AllocationId { get; set; }
        public int EmpId { get; set; }
        public string EmployeeName { get; set; } = string.Empty;
        public int ProjectId { get; set; }
        public string ProjectName { get; set; } = string.Empty;
        public DateTime AssignedOn { get; set; }
        public bool Status { get; set; }
        public string CreatedBY { get; set; } = string.Empty;
    }
}
