using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_LaptopShop.Application.DTOs
{
    public class BrandDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string? Slug { get; set; }
        public string? Description { get; set; }
        public string? Image { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
    }
    public class CreateBrandRequestDto
    {
        [Required(ErrorMessage = "Tên thương hiệu là bắt buộc.")]
        [StringLength(100, ErrorMessage = "Tên thương hiệu không được vượt quá 100 ký tự.")]
        public string Name { get; set; } = null!;

        [StringLength(200, ErrorMessage = "Slug không được vượt quá 200 ký tự.")]
        public string? Slug { get; set; } 
        [StringLength(500, ErrorMessage = "Mô tả không được vượt quá 500 ký tự.")]
        public string? Description { get; set; }

        [Url(ErrorMessage = "Đường dẫn ảnh logo không hợp lệ.")]
        [StringLength(500, ErrorMessage = "Đường dẫn ảnh logo không được vượt quá 500 ký tự.")]
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public bool IsActive { get; set; } = true; 
    }
    public class UpdateBrandRequestDto
    {
        [Required]
        public int Id { get; set; } 

        [StringLength(100, ErrorMessage = "Tên thương hiệu không được vượt quá 100 ký tự.")]
        public string Name { get; set; } = null!;

        [StringLength(200, ErrorMessage = "Slug không được vượt quá 200 ký tự.")]
        public string? Slug { get; set; }

        [StringLength(500, ErrorMessage = "Mô tả không được vượt quá 500 ký tự.")]
        public string? Description { get; set; }

        public bool IsActive { get; set; }
    }
}
