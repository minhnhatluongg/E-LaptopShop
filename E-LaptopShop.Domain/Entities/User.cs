using System;
using System.Collections.Generic;

namespace E_LaptopShop.Domain.Entities
{
    /// <summary>
    /// POCO entity — no EF attributes. See
    /// <c>Infra/Data/Configurations/UserConfiguration.cs</c>.
    /// </summary>
    public partial class User
    {
        public int Id { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;

        public string? PasswordHash { get; set; }
        public string? Phone { get; set; }
        public string? AvatarUrl { get; set; }
        public string? Token { get; set; }
        public string? RefreshToken { get; set; }
        public DateTime? RefreshTokenExpiryTime { get; set; }
        public DateTime? LastLoginAt { get; set; }

        public int LoginAttempts { get; set; } = 0;
        public bool IsLocked { get; set; } = false;
        public DateTime? LockedUntil { get; set; }

        public int RoleId { get; set; }

        public bool IsActive { get; set; } = true;
        public bool EmailConfirmed { get; set; } = false;
        public string? VerificationToken { get; set; }

        public string? Gender { get; set; }
        public DateTime? DateOfBirth { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
        public string? CreatedBy { get; set; }
        public string? UpdatedBy { get; set; }

        // Computed (not mapped to DB)
        public string FullName => $"{FirstName} {LastName}".Trim();

        // Navigation
        public virtual Role Role { get; set; } = null!;
        public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
        public virtual ICollection<ProductReview> ProductReviews { get; set; } = new List<ProductReview>();
        public virtual ICollection<UserAddress> UserAddresses { get; set; } = new List<UserAddress>();
        public virtual ICollection<ShoppingCart> ShoppingCarts { get; set; } = new List<ShoppingCart>();
        public virtual ICollection<CouponUsage> CouponUsages { get; set; } = new List<CouponUsage>();
        public virtual ICollection<Wishlist> Wishlists { get; set; } = new List<Wishlist>();
        public virtual ICollection<Notification> Notifications { get; set; } = new List<Notification>();
        public virtual ICollection<AuditLog> AuditLogs { get; set; } = new List<AuditLog>();
        public virtual ICollection<ActivityLog> ActivityLogs { get; set; } = new List<ActivityLog>();
        public virtual UserLoyalty? Loyalty { get; set; }
        public virtual ICollection<PointTransaction> PointTransactions { get; set; } = new List<PointTransaction>();
        public virtual ICollection<ReturnRequest> ReturnRequests { get; set; } = new List<ReturnRequest>();
        public virtual ICollection<Post> Posts { get; set; } = new List<Post>();
    }
}
