using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_LaptopShop.Application.Common.Exceptions
{
    public static class Throw
    {
        public static void BadRequest(string message, object? context = null,
            IReadOnlyDictionary<string, string[]>? errors = null, string? code = null)
            => throw new ValidationException(message, context, errors, code);

        public static void NotFound(string resourceType, object? resourceId = null)
        {
            var message = resourceId != null
                ? $"{resourceType} with ID '{resourceId}' not found"
                : $"{resourceType} not found";
            throw new NotFoundException(message, new { ResourceType = resourceType, ResourceId = resourceId });
        }

        public static void NotFound<T>(object? resourceId = null)
            => NotFound(typeof(T).Name, resourceId);

        public static void Forbidden(string resource, string action = "access")
            => throw new ForbiddenException(
                $"Access denied for action '{action}' on resource '{resource}'",
                new { Resource = resource, Action = action });

        public static void Conflict(string message, object? context = null)
            => throw new ConflictException(message, context);

        public static void BusinessRule(string rule, string message, object? context = null)
            => throw new BusinessRuleException(rule, message, context);

        public static void Unauthorized(string message = "Unauthorized access")
            => throw new UnauthorizedException(message);

        public static void If(bool condition, Action throwAction)
        {
            if (condition) throwAction();
        }

        public static void IfNull<T>(T? obj, string resourceType, object? resourceId = null) where T : class
        {
            if (obj is null) NotFound(resourceType, resourceId);
        }

        public static void IfEmpty<T>(IEnumerable<T>? collection, string message)
        {
            if (collection is null) BadRequest(message);

            if (collection is ICollection<T> col)
            {
                if (col.Count == 0) BadRequest(message);
            }
            else if (!collection.Any())
            {
                BadRequest(message);
            }
        }
        public static void IfNullOrEmpty(string? value, string fieldName)
        {
            if (string.IsNullOrWhiteSpace(value))
                BadRequest($"{fieldName} is required");
        }

        public static void IfNullOrNonPositive(int? value, string fieldName)
        {
            if (!value.HasValue || value <= 0)
                BadRequest($"{fieldName} must be greater than zero ", new { value });
        }
    }
}
