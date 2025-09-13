using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_LaptopShop.Application.Common.Pagination
{
    public class PaginationParams
    {
        private const int DefaultPageSize = 10;
        private const int MaxPageSize = 50;

        private int _pageSize = DefaultPageSize;
        public int _pageNumber = 1;

        [FromQuery(Name = "pageNumber")]
        [SwaggerParameter("Page number (starts from 1)", Required = false)]
        [Range(1, int.MaxValue, ErrorMessage = "Page number must be greater than 0")]
        public int PageNumber
        {
            get => _pageNumber;
            set => _pageNumber = value < 1 ? 1 : value;
        }

        [FromQuery(Name = "pageSize")]
        [SwaggerParameter("Number of items per page (max 50)", Required = false)]
        [Range(1, MaxPageSize, ErrorMessage = "Page size must be between 1 and 50")]
        public int PageSize
        {
            get => _pageSize;
            set => _pageSize = value > MaxPageSize ? MaxPageSize : value < 1 ? DefaultPageSize : value;
        }
    }
}
