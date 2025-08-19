using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_LaptopShop.Application.Common.Exceptions
{
    public static class Throw
    {
        /// <summary>
        /// Throw validation exception (400 Bad Request)
        /// Sử dụng khi input validation fails
        /// </summary>
        public static void BadRequest(string message, object? context = null)
        {
            throw new ValidationException(message, context);
        }

        /// <summary>
        /// Throw not found exception (404 Not Found)
        /// Tự động format message cho consistency
        /// </summary>
        public static void NotFound(string resourceType, object? resourceId = null)
        {
            var message = resourceId != null
                ? $"{resourceType} with ID '{resourceId}' not found"
                : $"{resourceType} not found";

            throw new NotFoundException(message, new { ResourceType = resourceType, ResourceId = resourceId });
        }

        /// <summary>
        /// Throw forbidden exception (403 Forbidden)
        /// Format chuẩn cho access denied messages
        /// </summary>
        public static void Forbidden(string resource, string action = "access")
        {
            var message = $"Access denied for action '{action}' on resource '{resource}'";
            throw new ForbiddenException(message, new { Resource = resource, Action = action });
        }

        /// <summary>
        /// Throw conflict exception (409 Conflict)
        /// Đơn giản hóa việc throw conflict
        /// </summary>
        public static void Conflict(string message, object? context = null)
        {
            throw new ConflictException(message, context);
        }

        /// <summary>
        /// Throw business rule exception (422 Unprocessable Entity)
        /// Bắt buộc phải có rule name để tracking
        /// </summary>
        public static void BusinessRule(string rule, string message, object? context = null)
        {
            throw new BusinessRuleException(rule, message, context);
        }

        /// <summary>
        /// Throw unauthorized exception (401 Unauthorized)
        /// </summary>
        public static void Unauthorized(string message = "Unauthorized access")
        {
            throw new UnauthorizedException(message);
        }

        /// <summary>
        /// Conditional throw - chỉ throw khi condition = true
        /// Giúp code ngắn gọn hơn
        /// </summary>
        public static void If(bool condition, Action throwAction)
        {
            if (condition) throwAction();
        }

        /// <summary>
        /// Throw if object is null - pattern phổ biến nhất
        /// Tự động format message và context
        /// </summary>
        public static void IfNull<T>(T? obj, string resourceType, object? resourceId = null) where T : class
        {
            if (obj == null) NotFound(resourceType, resourceId);
        }

        /// <summary>
        /// Throw if collection is empty
        /// Useful cho validation
        /// </summary>
        public static void IfEmpty<T>(IEnumerable<T>? collection, string message)
        {
            if (collection == null || !collection.Any()) BadRequest(message);
        }

        /// <summary>
        /// Throw if string is null or empty
        /// Common validation pattern
        /// </summary>
        public static void IfNullOrEmpty(string? value, string fieldName)
        {
            if (string.IsNullOrWhiteSpace(value))
                BadRequest($"{fieldName} is required");
        }
    }
}
