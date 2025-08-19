using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace E_LaptopShop.Application.Common.Exceptions
{
    public class BusinessRuleException : AppException
    {
        public string Rule { get; }

        public BusinessRuleException(string rule, string message, object? context = null)
            : base(HttpStatusCode.UnprocessableEntity, "BUSINESS_RULE_VIOLATION", message, context)
        {
            Rule = rule;
        }
    }
}
