using System.ComponentModel.DataAnnotations;

namespace E_LaptopShop.Application.DTOs
{
    public class BannerDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = null!;
        public string? Subtitle { get; set; }
        public string ImageUrl { get; set; } = null!;
        public string? LinkUrl { get; set; }
        public string Position { get; set; } = "HOMEPAGE_TOP";
        public int DisplayOrder { get; set; }
        public DateTime? StartsAt { get; set; }
        public DateTime? EndsAt { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class CreateBannerDto
    {
        [Required] [MaxLength(150)] public string Title { get; set; } = null!;
        [MaxLength(255)] public string? Subtitle { get; set; }
        [Required] [MaxLength(500)] public string ImageUrl { get; set; } = null!;
        [MaxLength(500)] public string? LinkUrl { get; set; }
        [MaxLength(50)] public string Position { get; set; } = "HOMEPAGE_TOP";
        public int DisplayOrder { get; set; } = 0;
        public DateTime? StartsAt { get; set; }
        public DateTime? EndsAt { get; set; }
        public bool IsActive { get; set; } = true;
    }

    public class UpdateBannerDto
    {
        [Required] public int Id { get; set; }
        [MaxLength(150)] public string? Title { get; set; }
        [MaxLength(255)] public string? Subtitle { get; set; }
        [MaxLength(500)] public string? ImageUrl { get; set; }
        [MaxLength(500)] public string? LinkUrl { get; set; }
        [MaxLength(50)] public string? Position { get; set; }
        public int? DisplayOrder { get; set; }
        public DateTime? StartsAt { get; set; }
        public DateTime? EndsAt { get; set; }
        public bool? IsActive { get; set; }
    }
}
