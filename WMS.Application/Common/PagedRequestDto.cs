using System.ComponentModel.DataAnnotations;

namespace WMS.Application.Common
{
    /// <summary>
    /// Standard pagination, search, and sort parameters used by all list endpoints.
    /// Every list API accepts these as query parameters — do not duplicate per module.
    /// </summary>
    public class PagedRequestDto
    {
        public int PageNumber { get; set; } = 1;

        [Range(1, 100)]
        public int PageSize { get; set; } = 10;

        public string? SearchTerm { get; set; }

        public string? SortBy { get; set; }

        // "asc" or "desc"
        public string? SortDirection { get; set; }
    }
}
