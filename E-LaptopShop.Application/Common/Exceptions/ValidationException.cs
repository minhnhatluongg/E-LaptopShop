using FluentValidation.Results;
using Microsoft.AspNetCore.Http;
using System.Net;

namespace E_LaptopShop.Application.Common.Exceptions
{
    public sealed class ValidationException : AppException
    {
        public ValidationException(string message, object? context = null,
            IReadOnlyDictionary<string, string[]>? errors = null, string? code = null)
            : base(StatusCodes.Status400BadRequest, message, code, context, errors) { }
    }
}
