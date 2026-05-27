using E_LaptopShop.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace E_LaptopShop.Infra.Data.Configurations
{
    public sealed class SupplierOrderConfiguration : IEntityTypeConfiguration<SupplierOrder>
    {
        public void Configure(EntityTypeBuilder<SupplierOrder> builder)
        {
            builder.ToTable("SupplierOrders");

            builder.HasKey(so => so.Id)
                   .HasName("PK__SupplierOrders__3214EC07");

            builder.Property(so => so.PurchaseNumber).IsRequired().HasMaxLength(50);
            builder.Property(so => so.Status).HasMaxLength(50).HasDefaultValue("Pending");
            builder.Property(so => so.TotalAmount).HasColumnType("decimal(18,2)").HasDefaultValue(0m);
            builder.Property(so => so.Notes).HasMaxLength(1000);
            builder.Property(so => so.CreatedBy).HasMaxLength(100);

            builder.Property(so => so.OrderDate)
                   .HasColumnType("datetime")
                   .HasDefaultValueSql("(getdate())");

            builder.Property(so => so.DeliveryDate).HasColumnType("datetime");

            builder.Property(so => so.CreatedAt)
                   .HasColumnType("datetime")
                   .HasDefaultValueSql("(getdate())");

            builder.Property(so => so.UpdatedAt).HasColumnType("datetime");

            builder.HasIndex(so => so.PurchaseNumber)
                   .IsUnique()
                   .HasDatabaseName("UX_SupplierOrders_PurchaseNumber");

            // Supplier relationship is configured in SupplierConfiguration (HasMany side)

            builder.HasMany(so => so.Items)
                   .WithOne(soi => soi.SupplierOrder)
                   .HasForeignKey(soi => soi.SupplierOrderId)
                   .OnDelete(DeleteBehavior.Cascade)
                   .HasConstraintName("FK_SupplierOrderItems_SupplierOrders");
        }
    }
}
