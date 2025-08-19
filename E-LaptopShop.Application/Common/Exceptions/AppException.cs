using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace E_LaptopShop.Application.Common.Exceptions
{
    public abstract class AppException : Exception
    {
        public HttpStatusCode StatusCode { get; }
        public string ErrorCode { get; }
        public object? ErrorContext { get; }

        protected AppException(
            HttpStatusCode statusCode, 
            string errorCode, 
            string message, 
            object? errorContext = null,
            Exception? innerException = null)
            : base(message, innerException)
        {
            StatusCode = statusCode;
            ErrorCode = errorCode;
            ErrorContext = errorContext;
        }
    }
}
