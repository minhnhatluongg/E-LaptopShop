using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace E_LaptopShop.Application.Common.Exceptions
{
    public sealed class ForbiddenException : AppException
    {
        public ForbiddenException(string message, object? context = null, string? code = null)
            : base(StatusCodes.Status403Forbidden, message, code, context) { }
    }
}
