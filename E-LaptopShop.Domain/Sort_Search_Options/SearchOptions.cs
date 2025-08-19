using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_LaptopShop.Application.Common.Pagination_Sort_Filter
{
    public class SearchOptions
    {
        public string? SearchTerm { get; set; }
        public string[]? SearchFields { get; set; }

        public bool HasSearch => !string.IsNullOrWhiteSpace(SearchTerm);
    }
}