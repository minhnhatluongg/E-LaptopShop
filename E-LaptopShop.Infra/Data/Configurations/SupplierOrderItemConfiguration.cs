using E_LaptopShop.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace E_LaptopShop.Infra.Data.Configurations
{
    public sealed class SupplierOrderItemConfiguration : IEntityTypeConfiguration<SupplierOrderItem>
    {
        public void Configure(EntityTypeBuilder<SupplierOrderItem> builder)
        {
            builder.ToTable("SupplierOrderItems");

            builder.HasKey(soi => soi.Id)
                   .HasName("PK__SupplierOrderItems__3214EC07");

            builder.Property(soi => soi.UnitCost).HasColumnType("decimal(18,2)");
            builder.Property(soi => soi.TotalCost).HasColumnType("decimal(18,2)");
            builder.Property(soi => soi.ReceivedQuantity).HasDefaultValue(0);
            builder.Property(soi => soi.Notes).HasMaxLength(500);

            // SupplierOrder relationship is configured in SupplierOrderConfiguration (HasMany side)

            builder.HasOne(soi => soi.Product)
                   .WithMany(p => p.SupplierOrderItems)
                   .HasForeignKey(soi => soi.ProductId)
                   .HasConstraintName("FK_SupplierOrderItems_Products");
        }
    }
}
