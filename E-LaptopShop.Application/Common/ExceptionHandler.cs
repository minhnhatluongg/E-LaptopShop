using System.Net;
using System.Text.Json;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace E_LaptopShop.Application.Common
{
    public class ExceptionHandler
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandler> _logger;

        public ExceptionHandler(RequestDelegate next, ILogger<ExceptionHandler> logger)
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
                await HandleExceptionAsync(context, ex);
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            var response = context.Response;
            response.ContentType = "application/json";

            var (statusCode, message) = GetStatusCodeAndMessage(exception);

            // Log the error
            _logger.LogError(exception, "An error occurred: {Message}", exception.Message);

            // Create error response
            var result = new
            {
                StatusCode = (int)statusCode,
                Message = message,
                DetailedMessage = exception.Message,
                Timestamp = DateTime.UtcNow
            };

            response.StatusCode = (int)statusCode;
            await response.WriteAsync(JsonSerializer.Serialize(result));
        }

        private (HttpStatusCode statusCode, string message) GetStatusCodeAndMessage(Exception exception)
        {
            // Handle database update exceptions
            if (exception is DbUpdateException dbUpdateEx)
            {
                // Check for foreign key violation
                if (dbUpdateEx.InnerException?.Message.Contains("foreign key") == true)
                {
                    return (HttpStatusCode.BadRequest, "The referenced record does not exist.");
                }

                // Check for unique constraint violation
                if (dbUpdateEx.InnerException?.Message.Contains("unique constraint") == true)
                {
                    return (HttpStatusCode.Conflict, "A record with this data already exists.");
                }

                return (HttpStatusCode.BadRequest, "Database update error occurred.");
            }

            if (exception is DbUpdateConcurrencyException)
                return (HttpStatusCode.Conflict, "Concurrency conflict occurred.");

            if (exception is KeyNotFoundException)
                return (HttpStatusCode.NotFound, exception.Message);

            if (exception is ArgumentNullException)
                return (HttpStatusCode.BadRequest, exception.Message);

            if (exception is ArgumentException)
                return (HttpStatusCode.BadRequest, exception.Message);

            // Handle InvalidOperationException with specific message
            if (exception is InvalidOperationException invalidOpEx &&
                invalidOpEx.Message.Contains("not found"))
                return (HttpStatusCode.NotFound, invalidOpEx.Message);

            // Handle any other exceptions
            return (HttpStatusCode.InternalServerError, "An unexpected error occurred.");
        }
    }

    // Extension method to add the exception handler to the pipeline
    public static class ExceptionHandlerExtensions
    {
        public static IApplicationBuilder UseGlobalExceptionHandler(this IApplicationBuilder app)
        {
            return app.UseMiddleware<ExceptionHandler>();
        }
    }
}