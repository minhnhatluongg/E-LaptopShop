using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_LaptopShop.Application.Common.Exceptions
{
    public class ConflictException : AppException
    {
        public ConflictException(string message, object? context = null)
            : base(System.Net.HttpStatusCode.Conflict, "RESOURCE_CONFLICT", message, context)
        {
        }
    }
}
