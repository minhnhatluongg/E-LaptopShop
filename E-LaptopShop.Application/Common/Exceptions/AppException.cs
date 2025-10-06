using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace E_LaptopShop.Application.Common.Exceptions
{
    // Base: chứa StatusCode + ErrorCode + Context + Errors (cho validation)
    public abstract class AppException : Exception
    {
        public int StatusCode { get; }
        public string? ErrorCode { get; }
        public object? Context { get; }
        public IReadOnlyDictionary<string, string[]>? Errors { get; }

        protected AppException(
            int statusCode, string message,
            string? errorCode = null,
            object? context = null,
            IReadOnlyDictionary<string, string[]>? errors = null,
            Exception? inner = null)
            : base(message, inner)
        {
            StatusCode = statusCode;
            ErrorCode = errorCode;
            Context = context;
            Errors = errors;
        }
    }

}
