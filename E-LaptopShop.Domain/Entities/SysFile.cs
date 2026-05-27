using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_LaptopShop.Domain.Entities
{
    public partial class SysFile
    {
        public int Id { get; set; }

        public string FileName { get; set; } = null!;

        public string FilePath { get; set; } = null!;

        public string FileUrl { get; set; } = null!;

        public string FileType { get; set; } = null!;

        public long FileSize { get; set; }

        public string StorageType { get; set; } = "local";

        public DateTime UploadedAt { get; set; } = DateTime.Now;

        public string? UploadedBy { get; set; }

        public bool IsActive { get; set; } = true;

        public virtual ICollection<ProductImage> ProductImages { get; set; } = new List<ProductImage>();
    }
}
