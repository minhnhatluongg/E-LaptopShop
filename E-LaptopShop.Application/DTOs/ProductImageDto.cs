using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_LaptopShop.Application.DTOs
{
    public class ProductImageDto
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public string ImageUrl { get; set; } = null!;
        public string FileType { get; set; } = null!;
        public long FileSize { get; set; }
        public bool IsMain { get; set; }
        public int DisplayOrder { get; set; }
        public string? AltText { get; set; }
        public string? Title { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime UpdatedDate { get; set; }
        public bool isActive { get; set; }
        public string CreatedBy { get; set; } = null!;

    }

    public class CreateProductImageDto 
    {
        public int ProductId { get; set; }
        public string ImageUrl { get; set; } = null!;
        public string FileType { get; set; } = null!;
        public long FileSize { get; set; }
        public bool IsMain { get; set; }
        public string? AltText { get; set; }
        public string? Title { get; set; }
    }

    public class UpdateProductImageDto
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public string ImageUrl { get; set; } = null!;
        public string FileType { get; set; } = null!;
        public long FileSize { get; set; }
        public bool IsMain { get; set; }
        public string? AltText { get; set; }
        public string? Title { get; set; }
    }
}
