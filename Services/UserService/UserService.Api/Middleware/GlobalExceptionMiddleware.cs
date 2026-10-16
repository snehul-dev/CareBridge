
using System.Net;
using CareBridge.Shared.Responses;
using CareBridge.Shared.Exceptions;

namespace UserService.Api.Middleware;

public sealed class GlobalExceptionMiddleware
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
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,"An exception occurred while processing {Path}",context.Request.Path);

            if (context.Response.HasStarted)
            {
                throw;
            }

            context.Response.Clear();

            var (statusCode, message) = ex switch
            {
                NotFoundException =>
                    (HttpStatusCode.NotFound, ex.Message),

                BadRequestException =>
                    (HttpStatusCode.BadRequest, ex.Message),

                UnauthorizedAccessException => 
                (HttpStatusCode.Forbidden , ex.Message),

                _ =>
                    (
                        HttpStatusCode.InternalServerError,
                        "An unexpected error occurred."
                    )
            };

            context.Response.StatusCode = (int)statusCode;
            context.Response.ContentType = "application/json";

            var response = ApiResponse<object>.Fail(
                new[] { message },
                message,
                statusCode);

            await context.Response.WriteAsJsonAsync(response,cancellationToken: context.RequestAborted);
        }
    }
}
