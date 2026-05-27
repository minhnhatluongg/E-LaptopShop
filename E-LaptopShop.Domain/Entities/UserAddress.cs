using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace E_LaptopShop.Domain.Entities;

public partial class UserAddress
{
    public int Id { get; set; }
    public int? UserId { get; set; }

    public string? FullName { get; set; }
    public string? Phone { get; set; }
    public string? AddressLine { get; set; }
    public string? City { get; set; }
    public string? District { get; set; }
    public string? Ward { get; set; }

    public bool IsDefault { get; set; } = false;
    public string CountryCode { get; set; } = "VN";
    public string? PostalCode { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTimeOffset? DeletedAt { get; set; }
    public bool IsDeleted { get; set; } = false;
    public DateTimeOffset? UpdatedAt { get; set; }

    public virtual User? User { get; set; }
    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
}

