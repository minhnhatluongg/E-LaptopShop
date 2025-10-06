using System;
using System.ComponentModel.DataAnnotations;

namespace E_LaptopShop.Application.DTOs
{
    /// <summary>
    /// ProductImage response DTO
    /// </summary>
    public class ProductImageDto
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public int? SysFileId { get; set; }
        public string ImageUrl { get; set; } = null!;
        public string FileType { get; set; } = null!;
        public long FileSize { get; set; }
        public bool IsMain { get; set; }
        public int DisplayOrder { get; set; }
        public string? AltText { get; set; }
        public string? Title { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UploadedAt { get; set; }
        public bool IsActive { get; set; }
        public string? CreatedBy { get; set; }

        // Navigation properties for response
        public string? ProductName { get; set; }
    }

    /// <summary>
    /// DTO for creating new ProductImage
    /// </summary>
    public class CreateProductImageRequestDto 
    {
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Product ID must be greater than zero")]
        public int ProductId { get; set; }

        public int? SysFileId { get; set; }

        [Required]
        [StringLength(255, ErrorMessage = "Image URL cannot exceed 255 characters")]
        public string ImageUrl { get; set; } = null!;

        [Required]
        [StringLength(50, ErrorMessage = "File type cannot exceed 50 characters")]
        public string FileType { get; set; } = null!;

        [Range(0, long.MaxValue, ErrorMessage = "File size must be non-negative")]
        public long FileSize { get; set; }

        public bool IsMain { get; set; } = false;

        [Range(0, int.MaxValue, ErrorMessage = "Display order must be non-negative")]
        public int DisplayOrder { get; set; } = 0;

        [StringLength(255, ErrorMessage = "Alt text cannot exceed 255 characters")]
        public string? AltText { get; set; }

        [StringLength(100, ErrorMessage = "Title cannot exceed 100 characters")]
        public string? Title { get; set; }

        [StringLength(50, ErrorMessage = "Created by cannot exceed 50 characters")]
        public string? CreatedBy { get; set; }
    }

    /// <summary>
    /// DTO for updating existing ProductImage
    /// </summary>
    public class UpdateProductImageRequestDto
    {
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "ID must be greater than zero")]
        public int Id { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Product ID must be greater than zero")]
        public int ProductId { get; set; }

        public int? SysFileId { get; set; }

        [Required]
        [StringLength(255, ErrorMessage = "Image URL cannot exceed 255 characters")]
        public string ImageUrl { get; set; } = null!;

        [Required]
        [StringLength(50, ErrorMessage = "File type cannot exceed 50 characters")]
        public string FileType { get; set; } = null!;

        [Range(0, long.MaxValue, ErrorMessage = "File size must be non-negative")]
        public long FileSize { get; set; }

        public bool IsMain { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Display order must be non-negative")]
        public int DisplayOrder { get; set; }

        [StringLength(255, ErrorMessage = "Alt text cannot exceed 255 characters")]
        public string? AltText { get; set; }

        [StringLength(100, ErrorMessage = "Title cannot exceed 100 characters")]
        public string? Title { get; set; }

        public bool IsActive { get; set; } = true;
    }

    /// <summary>
    /// Legacy DTOs for backward compatibility - DEPRECATED
    /// Use CreateProductImageRequestDto and UpdateProductImageRequestDto instead
    /// </summary>
    [Obsolete("Use CreateProductImageRequestDto instead")]
    public class CreateProductImageDto : CreateProductImageRequestDto { }

    [Obsolete("Use UpdateProductImageRequestDto instead")]
    public class UpdateProductImageDto : UpdateProductImageRequestDto { }
}
