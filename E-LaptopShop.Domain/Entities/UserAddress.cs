using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace E_LaptopShop.Domain.Entities;

public partial class UserAddress
{
    [Key]
    public int Id { get; set; }

    public int? UserId { get; set; }

    [StringLength(100)]
    public string? FullName { get; set; }

    [StringLength(20)]
    public string? Phone { get; set; }

    [StringLength(255)]
    public string? AddressLine { get; set; }

    [StringLength(100)]
    public string? City { get; set; }

    [StringLength(100)]
    public string? District { get; set; }

    [StringLength(100)]
    public string? Ward { get; set; }

    public bool? IsDefault { get; set; }

    [ForeignKey("UserId")]
    [InverseProperty("UserAddresses")]
    public virtual User? User { get; set; }
}
