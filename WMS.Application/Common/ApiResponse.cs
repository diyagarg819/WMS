namespace WMS.Application.Common
{
    /// <summary>
    /// Standard API response wrapper used by every controller endpoint.
    /// Ensures consistent JSON shape for both success and failure responses.
    /// </summary>
    public class ApiResponse<T>
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public T? Data { get; set; }
        public List<string> Errors { get; set; } = new List<string>();

        // Convenience constructor for success responses with data
        public ApiResponse(bool success, string message, T? data = default)
        {
            Success = success;
            Message = message;
            Data = data;
        }

        // Convenience constructor for failure responses with error list
        public ApiResponse(bool success, string message, List<string> errors)
        {
            Success = success;
            Message = message;
            Errors = errors;
        }
    }
}
