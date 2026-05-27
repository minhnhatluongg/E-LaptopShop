using System;
using System.Collections.Generic;

namespace E_LaptopShop.Domain.Entities
{
    /// <summary>
    /// POCO entity — no EF attributes. See
    /// <c>Infra/Data/Configurations/ProductConfiguration.cs</c>.
    /// </summary>
    public partial class Product
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string Slug { get; set; } = null!;
        public string? Description { get; set; }

        public decimal Price { get; set; }
        public decimal? Discount { get; set; }
        public int? InStock { get; set; }

        public int? CategoryId { get; set; }
        public int? BrandId { get; set; }

        public bool IsActive { get; set; } = true;
        public DateTime? CreatedAt { get; set; }

        // Navigation
        public virtual Category? Category { get; set; }
        public virtual Brand? Brand { get; set; }

        public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
        public virtual ICollection<ProductImage> ProductImages { get; set; } = new List<ProductImage>();
        public virtual ICollection<ProductReview> ProductReviews { get; set; } = new List<ProductReview>();
        public virtual ICollection<ProductSpecification> ProductSpecifications { get; set; } = new List<ProductSpecification>();
        public virtual ICollection<Inventory> Inventories { get; set; } = new List<Inventory>();
        public virtual ICollection<ShoppingCartItem> ShoppingCartItems { get; set; } = new List<ShoppingCartItem>();
        public virtual ICollection<SupplierOrderItem> SupplierOrderItems { get; set; } = new List<SupplierOrderItem>();
        public virtual ICollection<ProductTagMap> ProductTagMaps { get; set; } = new List<ProductTagMap>();
        public virtual ICollection<Wishlist> Wishlists { get; set; } = new List<Wishlist>();
        public virtual ICollection<ProductVariant> ProductVariants { get; set; } = new List<ProductVariant>();
    }
}
