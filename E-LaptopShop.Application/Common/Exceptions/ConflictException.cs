using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_LaptopShop.Application.Common.Exceptions
{
    public sealed class ConflictException : AppException
    {
        public ConflictException(string message, object? context = null, string? code = null)
            : base(StatusCodes.Status409Conflict, message, code, context) { }
    }
}
