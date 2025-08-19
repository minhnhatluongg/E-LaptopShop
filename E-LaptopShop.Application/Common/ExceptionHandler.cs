using E_LaptopShop.Application.Common.Exceptions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Text.Json;

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

            var errorResponse = CreateErrorResponse(exception);

            // Log với appropriate level
            if (errorResponse.StatusCode >= 500)
            {
                _logger.LogError(exception, "Server error: {Message}", exception.Message);
            }
            else if (errorResponse.StatusCode >= 400)
            {
                _logger.LogWarning("Client error: {Message}", exception.Message);
            }

            response.StatusCode = errorResponse.StatusCode;
            await response.WriteAsync(JsonSerializer.Serialize(errorResponse, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            }));
        }

        private ErrorResponse CreateErrorResponse(Exception exception)
        {
            return exception switch
            {
                // ✅ Handle custom AppException
                AppException appEx => new ErrorResponse
                {
                    Success = false,
                    StatusCode = (int)appEx.StatusCode,
                    ErrorCode = appEx.ErrorCode,
                    Message = appEx.Message,
                    Context = appEx.ErrorContext,
                    Errors = appEx is ValidationException validationEx ? validationEx.Errors : null,
                    Timestamp = DateTime.UtcNow
                },

                // ✅ Handle FluentValidation.ValidationException
                FluentValidation.ValidationException fluentEx => new ErrorResponse
                {
                    Success = false,
                    StatusCode = 400,
                    ErrorCode = "VALIDATION_FAILED",
                    Message = "Validation failed",
                    Errors = fluentEx.Errors?.GroupBy(e => e.PropertyName, e => e.ErrorMessage)
                        .Where(g => !string.IsNullOrEmpty(g.Key))
                        .ToDictionary(g => g.Key!, g => g.ToArray()),
                    Timestamp = DateTime.UtcNow
                },

                // ✅ Handle standard exceptions
                UnauthorizedAccessException => new ErrorResponse
                {
                    Success = false,
                    StatusCode = 401,
                    ErrorCode = "UNAUTHORIZED",
                    Message = exception.Message,
                    Timestamp = DateTime.UtcNow
                },

                KeyNotFoundException => new ErrorResponse
                {
                    Success = false,
                    StatusCode = 404,
                    ErrorCode = "RESOURCE_NOT_FOUND",
                    Message = exception.Message,
                    Timestamp = DateTime.UtcNow
                },

                ArgumentNullException or ArgumentException => new ErrorResponse
                {
                    Success = false,
                    StatusCode = 400,
                    ErrorCode = "INVALID_ARGUMENT",
                    Message = exception.Message,
                    Timestamp = DateTime.UtcNow
                },

                DbUpdateException dbEx when dbEx.InnerException?.Message.Contains("foreign key") == true => new ErrorResponse
                {
                    Success = false,
                    StatusCode = 400,
                    ErrorCode = "FOREIGN_KEY_VIOLATION",
                    Message = "The referenced record does not exist.",
                    Timestamp = DateTime.UtcNow
                },

                DbUpdateException dbEx when dbEx.InnerException?.Message.Contains("unique constraint") == true => new ErrorResponse
                {
                    Success = false,
                    StatusCode = 409,
                    ErrorCode = "UNIQUE_CONSTRAINT_VIOLATION",
                    Message = "A record with this data already exists.",
                    Timestamp = DateTime.UtcNow
                },

                DbUpdateException => new ErrorResponse
                {
                    Success = false,
                    StatusCode = 400,
                    ErrorCode = "DATABASE_ERROR",
                    Message = "Database update error occurred.",
                    Timestamp = DateTime.UtcNow
                },
                // ✅ Default case
                _ => new ErrorResponse
                {
                    Success = false,
                    StatusCode = 500,
                    ErrorCode = "INTERNAL_ERROR",
                    Message = "An unexpected error occurred.",
                    Timestamp = DateTime.UtcNow
                }
            };
        }
    }

    public class ErrorResponse
    {
        public bool Success { get; set; }
        public int StatusCode { get; set; }
        public string ErrorCode { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public object? Context { get; set; }
        public object? Errors { get; set; }
        public DateTime Timestamp { get; set; }
    }

    public static class ExceptionHandlerExtensions
    {
        public static IApplicationBuilder UseGlobalExceptionHandler(this IApplicationBuilder app)
        {
            return app.UseMiddleware<ExceptionHandler>();
        }
    }
}