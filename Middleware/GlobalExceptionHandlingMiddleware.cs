using System.Net;
using System.Text.Json;

namespace FootballLeagueApi.Middleware
{
    /// <summary>
    /// Global Exception Handling Middleware
    /// 
    /// This middleware catches all unhandled exceptions in the application
    /// and returns a standardized error response to the client.
    /// It logs the exception details for debugging and monitoring.
    /// </summary>
    public class GlobalExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<GlobalExceptionHandlingMiddleware> _logger;

        public GlobalExceptionHandlingMiddleware(RequestDelegate next, ILogger<GlobalExceptionHandlingMiddleware> logger)
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
                _logger.LogError(ex, "An unhandled exception occurred while processing the request: {Path} {Method}", 
                    context.Request.Path, context.Request.Method);
                
                await HandleExceptionAsync(context, ex);
            }
        }

        private static Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";
            var response = new ErrorResponse();

            switch (exception)
            {
                case KeyNotFoundException:
                    context.Response.StatusCode = StatusCodes.Status404NotFound;
                    response.Message = "The requested resource was not found.";
                    response.StatusCode = StatusCodes.Status404NotFound;
                    break;

                case ArgumentException:
                    context.Response.StatusCode = StatusCodes.Status400BadRequest;
                    response.Message = "Invalid argument provided.";
                    response.StatusCode = StatusCodes.Status400BadRequest;
                    break;

                case UnauthorizedAccessException:
                    context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    response.Message = "Unauthorized access.";
                    response.StatusCode = StatusCodes.Status401Unauthorized;
                    break;

                default:
                    context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                    response.Message = "An internal server error occurred. Please try again later.";
                    response.StatusCode = StatusCodes.Status500InternalServerError;
                    response.Details = exception.Message;
                    break;
            }

            return context.Response.WriteAsJsonAsync(response);
        }
    }

    /// <summary>
    /// Standard error response model returned by the API
    /// </summary>
    public class ErrorResponse
    {
        public int StatusCode { get; set; }
        public string Message { get; set; }
        public string? Details { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    }
}
