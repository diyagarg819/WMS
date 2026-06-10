using WMS.Application.Common;

namespace WMS.Application.DTOs.Leave
{
    /// <summary>
    /// Filter parameters for leave history queries — extends the base paged request.
    /// Allows filtering by Status.
    /// </summary>
    public class LeaveFilterDto : PagedRequestDto
    {
        public string? Status { get; set; }
    }
}
