using E_LaptopShop.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace E_LaptopShop.Infra.Data.Configurations
{
    public sealed class CouponUsageConfiguration : IEntityTypeConfiguration<CouponUsage>
    {
        public void Configure(EntityTypeBuilder<CouponUsage> builder)
        {
            builder.ToTable("CouponUsages");

            builder.HasKey(cu => cu.Id);

            builder.Property(cu => cu.AmountSaved).HasColumnType("decimal(18,2)");

            builder.Property(cu => cu.UsedAt)
                   .HasDefaultValueSql("(sysutcdatetime())");

            builder.HasIndex(cu => cu.UserId)
                   .HasDatabaseName("IX_CouponUsages_User");

            // Coupon relationship is configured in CouponConfiguration (HasMany side)

            builder.HasOne(cu => cu.User)
                   .WithMany(u => u.CouponUsages)
                   .HasForeignKey(cu => cu.UserId)
                   .OnDelete(DeleteBehavior.SetNull);

            builder.HasOne(cu => cu.Order)
                   .WithMany(o => o.CouponUsages)
                   .HasForeignKey(cu => cu.OrderId)
                   .OnDelete(DeleteBehavior.SetNull);
        }
    }
}
