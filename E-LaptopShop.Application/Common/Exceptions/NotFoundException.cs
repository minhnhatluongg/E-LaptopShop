using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_LaptopShop.Application.Common.Exceptions
{
    public class NotFoundException : AppException
    {
        public NotFoundException(string message, object? context = null)
            : base(System.Net.HttpStatusCode.NotFound, "NOT_FOUND", message, context)
        {
        }
    }
}
