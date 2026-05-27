using E_LaptopShop.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace E_LaptopShop.Infra.Data.Configurations
{
    public sealed class WishlistConfiguration : IEntityTypeConfiguration<Wishlist>
    {
        public void Configure(EntityTypeBuilder<Wishlist> builder)
        {
            builder.ToTable("Wishlists");

            builder.HasKey(w => w.Id);

            builder.Property(w => w.AddedAt)
                   .HasDefaultValueSql("(sysutcdatetime())");

            builder.HasIndex(w => new { w.UserId, w.ProductId })
                   .IsUnique()
                   .HasDatabaseName("UX_Wishlists_User_Product");

            builder.HasOne(w => w.User)
                   .WithMany(u => u.Wishlists)
                   .HasForeignKey(w => w.UserId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(w => w.Product)
                   .WithMany(p => p.Wishlists)
                   .HasForeignKey(w => w.ProductId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
