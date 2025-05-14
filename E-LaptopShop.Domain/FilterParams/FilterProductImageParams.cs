using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_LaptopShop.Domain.FilterParams
{
    public class ProductImageFilterParams
    {
        public int? Id { get; set; }
        public int? ProductId { get; set; }
        public string? ImageUrl { get; set; }
        public bool? IsMain { get; set; }
        public string? FileType { get; set; }
        public long? FileSize { get; set; }
        public int? DisplayOrder { get; set; }
        public string? AltText { get; set; }
        public string? Title { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UploadedAt { get; set; }
        public bool? IsActive { get; set; }
        public string? CreatedBy { get; set; }
    }
}
