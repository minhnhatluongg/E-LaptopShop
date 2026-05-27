using E_LaptopShop.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace E_LaptopShop.Infra.Data.Configurations
{
    public sealed class InventoryHistoryConfiguration : IEntityTypeConfiguration<InventoryHistory>
    {
        public void Configure(EntityTypeBuilder<InventoryHistory> builder)
        {
            builder.ToTable("InventoryHistories");

            builder.HasKey(ih => ih.Id)
                   .HasName("PK__InventoryHistories__3214EC07");

            builder.Property(ih => ih.TransactionType).IsRequired().HasMaxLength(50);
            builder.Property(ih => ih.UnitCost).HasColumnType("decimal(18,2)");
            builder.Property(ih => ih.ReferenceType).HasMaxLength(50);
            builder.Property(ih => ih.Notes).HasMaxLength(500);
            builder.Property(ih => ih.CreatedBy).HasMaxLength(100);

            builder.Property(ih => ih.TransactionDate)
                   .HasColumnType("datetime")
                   .HasDefaultValueSql("(getdate())");

            // Inventory relationship is configured in InventoryConfiguration (HasMany side)
        }
    }
}
