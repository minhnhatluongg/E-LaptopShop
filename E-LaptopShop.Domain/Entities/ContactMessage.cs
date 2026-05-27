using System;
using System.Collections.Generic;

namespace E_LaptopShop.Domain.Entities;

public partial class ContactMessage
{
    public int Id { get; set; }

    public string FullName { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string? Phone { get; set; }

    public string? Subject { get; set; }

    public string Message { get; set; } = null!;

    public bool IsHandled { get; set; }

    public string? HandledBy { get; set; }

    public DateTime? HandledAt { get; set; }

    public DateTime CreatedAt { get; set; }
}
