namespace WMS.Application.Common
{
    /// <summary>
    /// Standard pagination response wrapper used by all list endpoints.
    /// Contains the data page plus metadata for building pagination controls.
    /// </summary>
    public class PagedResponseDto<T>
    {
        public List<T> Data { get; set; } = new();
        public int TotalCount { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
    }
}
