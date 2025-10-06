using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_LaptopShop.Domain.FilterParams
{
    public class UserAddressFilterParams
    {
        public int? UserId { get; set; }
        public bool? IsDefault { get; set; }
        public string? CountryCode { get; set; } = "VN";
        public bool? IsDeleted { get; set; } = false;
        public string? Ward { get; set; }
        public string? City { get; set; }

        public DateTimeOffset? CreatedFrom { get; set; }
        public DateTimeOffset? CreatedTo { get; set; }
        public DateTimeOffset? UpdatedFrom { get; set; }
        public DateTimeOffset? UpdatedTo { get; set; }
    }
}