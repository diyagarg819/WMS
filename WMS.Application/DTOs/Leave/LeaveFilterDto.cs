using WMS.Application.Common;

namespace WMS.Application.DTOs.Leave
{
    public class LeaveFilterDto : SearchRequestDto
    {
        public string? Status { get; set; }
    }
}
