
using CareBridge.Shared.Exceptions;
using CareBridge.Shared.Responses;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Text.Json;

namespace CareBridge.Shared.Middlewares;

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
        catch (OperationCanceledException)
            when (context.RequestAborted.IsCancellationRequested)
        {
            // The client cancelled the request.
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "An unhandled exception occurred while processing {Path}",
                context.Request.Path);

            if (context.Response.HasStarted)
                throw;

            var statusCode = ex switch
            {
                BadRequestException =>
                    HttpStatusCode.BadRequest,

                NotFoundException =>
                    HttpStatusCode.NotFound,

                UnauthorizedException =>
                    HttpStatusCode.Unauthorized,

                ArgumentException =>
                    HttpStatusCode.BadRequest,

                AppointmentSlotConflictException =>
                    HttpStatusCode.Conflict,

                InvalidOperationException =>
                    HttpStatusCode.BadRequest,

                _ => HttpStatusCode.InternalServerError
            };

            var message =
                statusCode == HttpStatusCode.InternalServerError
                    ? "An unexpected error occurred."
                    : ex.Message;

            var response = ApiResponse<object>.Fail(
                new[] { message },
                message,
                statusCode);

            context.Response.Clear();
            context.Response.StatusCode = (int)statusCode;
            context.Response.ContentType = "application/json";

            await context.Response.WriteAsync(
                JsonSerializer.Serialize(response),
                context.RequestAborted);
        }
    }
}