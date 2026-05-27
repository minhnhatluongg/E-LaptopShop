using E_LaptopShop.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace E_LaptopShop.Infra.Data.Configurations
{
    public sealed class ProductVariantConfiguration : IEntityTypeConfiguration<ProductVariant>
    {
        public void Configure(EntityTypeBuilder<ProductVariant> builder)
        {
            builder.ToTable("ProductVariants");

            builder.HasKey(pv => pv.Id);

            builder.Property(pv => pv.SKU).IsRequired().HasMaxLength(100);
            builder.Property(pv => pv.Price).HasColumnType("decimal(18,2)");
            builder.Property(pv => pv.CompareAtPrice).HasColumnType("decimal(18,2)");
            builder.Property(pv => pv.CostPrice).HasColumnType("decimal(18,2)");
            builder.Property(pv => pv.Barcode).HasMaxLength(80);
            builder.Property(pv => pv.IsActive).HasDefaultValue(true);

            builder.Property(pv => pv.CreatedAt)
                   .HasDefaultValueSql("(sysutcdatetime())");

            builder.HasIndex(pv => pv.ProductId)
                   .HasDatabaseName("IX_ProductVariants_ProductId");

            builder.HasIndex(pv => pv.SKU)
                   .IsUnique()
                   .HasDatabaseName("UX_ProductVariants_SKU");

            builder.HasOne(pv => pv.Product)
                   .WithMany(p => p.ProductVariants)
                   .HasForeignKey(pv => pv.ProductId)
                   .HasConstraintName("FK_ProductVariants_Products");

            builder.HasMany(pv => pv.AttributeValue)
                   .WithMany(pav => pav.Variant)
                   .UsingEntity<Dictionary<string, object>>(
                       "ProductVariantValueMap",
                       r => r.HasOne<ProductAttributeValue>().WithMany()
                             .HasForeignKey("AttributeValueId")
                             .OnDelete(DeleteBehavior.ClientSetNull)
                             .HasConstraintName("FK_PVVM_Values"),
                       l => l.HasOne<ProductVariant>().WithMany()
                             .HasForeignKey("VariantId")
                             .HasConstraintName("FK_PVVM_Variants"),
                       j => j.HasKey("VariantId", "AttributeValueId"));
        }
    }
}
