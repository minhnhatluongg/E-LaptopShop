using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace E_LaptopShop.Application.Common.Exceptions
{
    public sealed class BusinessRuleException : AppException
    {
        public BusinessRuleException(string rule, string message, object? context = null, string? code = null)
            : base(StatusCodes.Status422UnprocessableEntity, $"{rule}: {message}", code, context) { }
    }
}
