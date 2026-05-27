using E_LaptopShop.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace E_LaptopShop.Infra.Data.Configurations
{
    public sealed class OrderItemConfiguration : IEntityTypeConfiguration<OrderItem>
    {
        public void Configure(EntityTypeBuilder<OrderItem> builder)
        {
            builder.ToTable("OrderItems");

            builder.HasKey(oi => oi.Id)
                   .HasName("PK__OrderIte__3214EC079E5D0449");

            builder.Property(oi => oi.UnitPrice).HasColumnType("decimal(18,2)");
            builder.Property(oi => oi.CostPrice).HasColumnType("decimal(18,2)").HasDefaultValue(0m);
            builder.Property(oi => oi.DiscountAmount).HasColumnType("decimal(18,2)").HasDefaultValue(0m);
            builder.Property(oi => oi.DiscountPercent).HasColumnType("decimal(18,2)").HasDefaultValue(0m);
            builder.Property(oi => oi.TaxAmount).HasColumnType("decimal(18,2)").HasDefaultValue(0m);
            builder.Property(oi => oi.SubTotal).HasColumnType("decimal(18,2)");
            builder.Property(oi => oi.SKU).HasMaxLength(100);
            builder.Property(oi => oi.SerialNumber).HasMaxLength(100);
            builder.Property(oi => oi.Status).HasMaxLength(50).HasDefaultValue("Pending");
            builder.Property(oi => oi.Notes).HasMaxLength(500);

            // Order relationship is configured in OrderConfiguration (HasMany side)

            builder.HasOne(oi => oi.Product)
                   .WithMany(p => p.OrderItems)
                   .HasForeignKey(oi => oi.ProductId)
                   .HasConstraintName("FK__OrderItem__Produ__52593CB8");

            builder.HasMany(oi => oi.ReturnRequests)
                   .WithOne(rr => rr.OrderItem)
                   .HasForeignKey(rr => rr.OrderItemId)
                   .OnDelete(DeleteBehavior.SetNull);
        }
    }
}
