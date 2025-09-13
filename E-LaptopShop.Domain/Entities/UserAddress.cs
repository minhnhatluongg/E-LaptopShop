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

    [StringLength(100)] public string? FullName { get; set; }
    [StringLength(20)] public string? Phone { get; set; }
    [StringLength(255)] public string? AddressLine { get; set; }
    [StringLength(100)] public string? City { get; set; }
    [StringLength(100)] public string? District { get; set; }
    [StringLength(100)] public string? Ward { get; set; }

    public bool IsDefault { get; set; } = false;
    [StringLength(50)] public string CountryCode { get; set; } = "VN";
    [StringLength(30)] public string? PostalCode { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTimeOffset? DeletedAt { get; set; } 
    public bool IsDeleted { get; set; } = false;
    public DateTimeOffset? UpdatedAt { get; set; }

    [ForeignKey("UserId")]
    [InverseProperty("UserAddresses")]
    public virtual User? User { get; set; }
}

