using E_LaptopShop.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace E_LaptopShop.Infra.Data.Configurations
{
    public sealed class InventoryConfiguration : IEntityTypeConfiguration<Inventory>
    {
        public void Configure(EntityTypeBuilder<Inventory> builder)
        {
            builder.ToTable("Inventories");

            builder.HasKey(i => i.Id)
                   .HasName("PK__Inventories__3214EC07");

            builder.Property(i => i.CurrentStock).HasDefaultValue(0);
            builder.Property(i => i.MinimumStock).HasDefaultValue(5);
            builder.Property(i => i.ReorderPoint).HasDefaultValue(10);

            builder.Property(i => i.AverageCost)
                   .HasColumnType("decimal(18,2)")
                   .HasDefaultValue(0m);

            builder.Property(i => i.LastPurchasePrice)
                   .HasColumnType("decimal(18,2)")
                   .HasDefaultValue(0m);

            builder.Property(i => i.LastUpdated)
                   .HasColumnType("datetime")
                   .HasDefaultValueSql("(getdate())");

            builder.Property(i => i.Location).HasMaxLength(100);

            builder.HasIndex(i => i.ProductId).HasDatabaseName("IX_Inventories_ProductId");

            builder.HasOne(i => i.Product)
                   .WithMany(p => p.Inventories)
                   .HasForeignKey(i => i.ProductId)
                   .OnDelete(DeleteBehavior.Cascade)
                   .HasConstraintName("FK_Inventories_Products");

            builder.HasMany(i => i.HistoryRecords)
                   .WithOne(ih => ih.Inventory)
                   .HasForeignKey(ih => ih.InventoryId)
                   .OnDelete(DeleteBehavior.Cascade)
                   .HasConstraintName("FK_InventoryHistories_Inventories");
        }
    }
}
