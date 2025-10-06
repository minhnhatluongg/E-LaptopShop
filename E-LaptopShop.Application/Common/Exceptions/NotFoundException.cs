using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_LaptopShop.Application.Common.Exceptions
{
    public sealed class NotFoundException : AppException
    {
        public NotFoundException(string message, object? context = null, string? code = null)
            : base(StatusCodes.Status404NotFound, message, code, context) { }
    }
}
