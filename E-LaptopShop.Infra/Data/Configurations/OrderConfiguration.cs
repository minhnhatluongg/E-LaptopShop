using E_LaptopShop.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace E_LaptopShop.Infra.Data.Configurations
{
    public sealed class OrderConfiguration : IEntityTypeConfiguration<Order>
    {
        public void Configure(EntityTypeBuilder<Order> builder)
        {
            builder.ToTable("Orders");

            builder.HasKey(o => o.Id)
                   .HasName("PK__Orders__3214EC07B7B3B259");

            builder.Property(o => o.OrderNumber)
                   .IsRequired()
                   .HasMaxLength(50);

            builder.Property(o => o.OrderDate)
                   .HasColumnType("datetime")
                   .HasDefaultValueSql("(getdate())");

            builder.Property(o => o.Status)
                   .HasMaxLength(50)
                   .HasDefaultValue("Pending");

            // Money columns
            builder.Property(o => o.SubTotal).HasColumnType("decimal(18,2)").HasDefaultValue(0m);
            builder.Property(o => o.DiscountAmount).HasColumnType("decimal(18,2)").HasDefaultValue(0m);
            builder.Property(o => o.TaxAmount).HasColumnType("decimal(18,2)").HasDefaultValue(0m);
            builder.Property(o => o.ShippingFee).HasColumnType("decimal(18,2)").HasDefaultValue(0m);
            builder.Property(o => o.TotalAmount).HasColumnType("decimal(18,2)");

            builder.Property(o => o.DiscountCode).HasMaxLength(100);
            builder.Property(o => o.ShippingMethod).HasMaxLength(50);
            builder.Property(o => o.PaymentMethod).HasMaxLength(50);
            builder.Property(o => o.IsPaid).HasDefaultValue(false);
            builder.Property(o => o.PaidDate).HasColumnType("datetime");

            builder.Property(o => o.Notes).HasMaxLength(1000);

            builder.Property(o => o.CreatedAt)
                   .HasColumnType("datetime")
                   .HasDefaultValueSql("(getdate())");
            builder.Property(o => o.UpdatedAt).HasColumnType("datetime");
            builder.Property(o => o.CreatedBy).HasMaxLength(100);
            builder.Property(o => o.UpdatedBy).HasMaxLength(100);

            // Indexes
            builder.HasIndex(o => o.OrderNumber).IsUnique().HasDatabaseName("UX_Orders_OrderNumber");
            builder.HasIndex(o => o.UserId).HasDatabaseName("IX_Orders_UserId");
            builder.HasIndex(o => o.Status).HasDatabaseName("IX_Orders_Status");
            builder.HasIndex(o => o.OrderDate).HasDatabaseName("IX_Orders_OrderDate");

            // Relationships
            builder.HasOne(o => o.User)
                   .WithMany(u => u.Orders)
                   .HasForeignKey(o => o.UserId)
                   .HasConstraintName("FK__Orders__UserId__4E88ABD4");

            // ShippingAddress relationship is configured in UserAddressConfiguration (HasMany side)

            builder.HasMany(o => o.OrderItems)
                   .WithOne(oi => oi.Order)
                   .HasForeignKey(oi => oi.OrderId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(o => o.OrderHistories)
                   .WithOne(oh => oh.Order)
                   .HasForeignKey(oh => oh.OrderId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(o => o.PaymentTransactions)
                   .WithOne(p => p.Order)
                   .HasForeignKey(p => p.OrderId);
        }
    }
}
