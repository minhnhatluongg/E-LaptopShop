using FluentValidation.Results;
using System.Net;

namespace E_LaptopShop.Application.Common.Exceptions
{
    public class ValidationException : AppException
    {
        public IDictionary<string, string[]> Errors { get; }
        public ValidationException(string message, object? context = null) : base(HttpStatusCode.BadRequest, "VALIDATION_FAILED", message, context)
        {
            Errors = new Dictionary<string, string[]>();
        }
        public ValidationException(IEnumerable<ValidationFailure> failures)
            : base(HttpStatusCode.BadRequest, "VALIDATION_FAILED", "One or more validation failures have occurred.")
        {
            Errors = failures
                .GroupBy(e => e.PropertyName, e => e.ErrorMessage)
                .Where(g => !string.IsNullOrEmpty(g.Key))
                .ToDictionary(g => g.Key!, g => g.ToArray());
        }
    }
}
