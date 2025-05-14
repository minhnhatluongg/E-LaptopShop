using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_LaptopShop.Application.Common.Pagination_Sort_Filter
{
    public class SortingOptions
    {
        public string? SortBy { get; set; }
        public bool IsAscending { get; set; } = true;

        public static SortingOptions Default => new() { SortBy = null, IsAscending = true };

        public bool HasSorting => !string.IsNullOrWhiteSpace(SortBy);
    }
}
