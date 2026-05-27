using E_LaptopShop.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace E_LaptopShop.Infra.Data.Configurations
{
    public sealed class CouponConfiguration : IEntityTypeConfiguration<Coupon>
    {
        public void Configure(EntityTypeBuilder<Coupon> builder)
        {
            builder.ToTable("Coupons");

            builder.HasKey(c => c.Id);

            builder.Property(c => c.Code).IsRequired().HasMaxLength(50);
            builder.Property(c => c.Description).HasMaxLength(255);
            builder.Property(c => c.DiscountType).IsRequired().HasMaxLength(20).HasDefaultValue("PERCENT");
            builder.Property(c => c.DiscountValue).HasColumnType("decimal(18,2)");
            builder.Property(c => c.MinOrderAmount).HasColumnType("decimal(18,2)");
            builder.Property(c => c.MaxDiscountAmount).HasColumnType("decimal(18,2)");
            builder.Property(c => c.CreatedBy).HasMaxLength(100);
            builder.Property(c => c.IsActive).HasDefaultValue(true);

            builder.Property(c => c.StartsAt)
                   .HasDefaultValueSql("(sysutcdatetime())");

            builder.Property(c => c.CreatedAt)
                   .HasDefaultValueSql("(sysutcdatetime())");

            builder.HasIndex(c => c.Code)
                   .IsUnique()
                   .HasDatabaseName("UX_Coupons_Code");

            builder.HasMany(c => c.CouponUsages)
                   .WithOne(cu => cu.Coupon)
                   .HasForeignKey(cu => cu.CouponId)
                   .HasConstraintName("FK_CouponUsages_Coupons");
        }
    }
}
