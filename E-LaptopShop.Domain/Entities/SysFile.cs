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
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(255)]
        public string FileName { get; set; } = null!;

        [Required]
        [StringLength(255)]
        public string FilePath { get; set; } = null!;

        [Required]
        [StringLength(255)]
        public string FileUrl { get; set; } = null!;

        [Required]
        [StringLength(50)]
        public string FileType { get; set; } = null!;

        public long FileSize { get; set; }
        [Required]
        [StringLength(100)]
        public string StorageType { get; set; } = "local";

        [Column(TypeName = "datetime")]
        public DateTime UploadedAt { get; set; } = DateTime.Now;

        [StringLength(50)]
        public string? UploadedBy { get; set; }

        public bool IsActive { get; set; } = true;

        [InverseProperty("SysFile")]
        public virtual ICollection<ProductImage> ProductImages { get; set; } = new List<ProductImage>();
    }
}
