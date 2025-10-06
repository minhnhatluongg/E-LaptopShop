using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace E_LaptopShop.Application.Common.Exceptions
{
    public sealed class UnauthorizedException : AppException
    {
        public UnauthorizedException(string message = "Unauthorized", string? code = null, object? context = null)
            : base(StatusCodes.Status401Unauthorized, message, code, context) { }
    }
}
