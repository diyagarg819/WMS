using System.Net;
using System.Text.Json;
using WMS.Application.Common;

namespace WMS.Api.Middleware
{
    /// <summary>
    /// Catches all unhandled exceptions and returns a consistent API response.
    /// This replaces the need for try/catch in every controller endpoint.
    /// Never exposes stack traces to the client.
    /// </summary>
    public class GlobalExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<GlobalExceptionMiddleware> _logger;

        public GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                // Log every unhandled exception before returning a response
                _logger.LogError(ex, "An unhandled exception occurred while processing the request");
                await HandleExceptionAsync(context, ex);
            }
        }

        // Map the exception to an appropriate HTTP status code and response body
        private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";

            var statusCode = exception switch
            {
                UnauthorizedAccessException => (int)HttpStatusCode.Unauthorized,
                KeyNotFoundException => (int)HttpStatusCode.NotFound,
                ArgumentException => (int)HttpStatusCode.BadRequest,
                _ => (int)HttpStatusCode.InternalServerError
            };

            context.Response.StatusCode = statusCode;

            // Use a generic message for 500 errors — never expose internal details
            string message;
            if (statusCode == (int)HttpStatusCode.InternalServerError)
                message = "An unexpected error occurred. Please try again later.";
            else
                message = exception.Message;

            var response = new ApiResponse<object>(false, message);

            var jsonOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };

            string json = JsonSerializer.Serialize(response, jsonOptions);
            await context.Response.WriteAsync(json);
        }
    }
}
