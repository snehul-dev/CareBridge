using System.Text.Json;
using UserService.Application.Exceptions;

namespace UserService.Api.Middleware
{
    public class GlobalExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<GlobalExceptionMiddleware> _logger;

        public GlobalExceptionMiddleware(
            RequestDelegate next,
            ILogger<GlobalExceptionMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                // Pass the request to the next middleware
                await _next(context);
            }
            catch (Exception ex)
            {
                // Log the exception for developers
                _logger.LogError(
                    ex,
                    "An exception occurred while processing {Path}",
                    context.Request.Path);

                // Cannot replace a response that has already started
                if (context.Response.HasStarted)
                {
                    throw;
                }

                // Clear the existing response
                context.Response.Clear();

                // Determine the HTTP status code
                var statusCode = ex switch
                {
                    NotFoundException =>
                        StatusCodes.Status404NotFound,

                    BadRequestException =>
                        StatusCodes.Status400BadRequest,

                    _ =>
                        StatusCodes.Status500InternalServerError
                };

                // Set a safe error message
                var message = ex switch
                {
                    NotFoundException => ex.Message,
                    BadRequestException => ex.Message,
                    _ => "An unexpected error occurred."
                };

                // Set the response status and content type
                context.Response.StatusCode = statusCode;
                context.Response.ContentType = "application/json";

                // Create a consistent error response
                var response = new
                {
                    statusCode = statusCode,
                    message = message,
                    traceId = context.TraceIdentifier
                };

                // Send the JSON response to the client
                await context.Response.WriteAsync(
                    JsonSerializer.Serialize(response));
            }
        }
    }
}