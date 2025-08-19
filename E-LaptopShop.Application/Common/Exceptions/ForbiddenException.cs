using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace E_LaptopShop.Application.Common.Exceptions
{
    public class ForbiddenException : AppException
    {
        public ForbiddenException(string message, object? context = null)
            : base(HttpStatusCode.Forbidden, "ACCESS_DENIED", message, context)
        {

        }
    }
}
