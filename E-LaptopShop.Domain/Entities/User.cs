using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace E_LaptopShop.Domain.Entities;

[Index("Email", Name = "UQ__Users__A9D10534BCF0AE75", IsUnique = true)]
[Index("RoleId", Name = "IX_Users_RoleId")]
[Index("IsActive", Name = "IX_Users_IsActive")]
[Index("LastLoginAt", Name = "IX_Users_LastLoginAt")]
public partial class User
{
    [Key]
    public int Id { get; set; }

    [StringLength(50)]
    [Required]
    public string FirstName { get; set; } = string.Empty;

    [StringLength(50)]
    [Required]
    public string LastName { get; set; } = string.Empty;

    [StringLength(100)]
    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [StringLength(255)]
    public string? PasswordHash { get; set; }

    [StringLength(20)]
    public string? Phone { get; set; }

    [StringLength(255)]
    public string? AvatarUrl { get; set; }

    [StringLength(255)]
    public string? Token { get; set; }

    [StringLength(512)]
    public string? RefreshToken { get; set; }

    public DateTime? RefreshTokenExpiryTime { get; set; }

    public DateTime? LastLoginAt { get; set; }

    public int LoginAttempts { get; set; } = 0;

    public bool IsLocked { get; set; } = false;

    public DateTime? LockedUntil { get; set; }

    [Required]
    public int RoleId { get; set; }

    public bool IsActive { get; set; } = true;

    public bool EmailConfirmed { get; set; } = false;

    [StringLength(100)]
    public string? VerificationToken { get; set; }

    [StringLength(50)]
    public string? Gender { get; set; }

    public DateTime? DateOfBirth { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [Column(TypeName = "datetime")]
    public DateTime? UpdatedAt { get; set; }

    [StringLength(100)]
    public string? CreatedBy { get; set; }

    [StringLength(100)]
    public string? UpdatedBy { get; set; }

    [NotMapped]
    public string FullName => $"{FirstName} {LastName}".Trim();

    [InverseProperty("User")]
    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();

    [InverseProperty("User")]
    public virtual ICollection<ProductReview> ProductReviews { get; set; } = new List<ProductReview>();

    [ForeignKey("RoleId")]
    [InverseProperty("Users")]
    public virtual Role Role { get; set; } = null!;

    [InverseProperty("User")]
    public virtual ICollection<UserAddress> UserAddresses { get; set; } = new List<UserAddress>();
}
