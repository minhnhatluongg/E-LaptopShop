using System;
using System.Collections.Generic;

namespace E_LaptopShop.Domain.Entities;

public partial class Post
{
    public int Id { get; set; }

    public int? AuthorUserId { get; set; }

    public string Title { get; set; } = null!;

    public string Slug { get; set; } = null!;

    public string? Excerpt { get; set; }

    public string? Content { get; set; }

    public string? CoverImageUrl { get; set; }

    public string Status { get; set; } = null!;

    public DateTime? PublishedAt { get; set; }

    public int ViewCount { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    // Navigation
    public virtual User? Author { get; set; }
}
