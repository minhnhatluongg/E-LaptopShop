using System;
using System.Collections.Generic;

namespace E_LaptopShop.Domain.Entities;

public partial class Banner
{
    public int Id { get; set; }

    public string Title { get; set; } = null!;

    public string? Subtitle { get; set; }

    public string ImageUrl { get; set; } = null!;

    public string? LinkUrl { get; set; }

    public string Position { get; set; } = null!;

    public int DisplayOrder { get; set; }

    public DateTime? StartsAt { get; set; }

    public DateTime? EndsAt { get; set; }

    public bool IsActive { get; set; }

    public DateTime CreatedAt { get; set; }
}
