using E_LaptopShop.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace E_LaptopShop.Infra.Data.Configurations
{
    public sealed class ProductAttributeValueConfiguration : IEntityTypeConfiguration<ProductAttributeValue>
    {
        public void Configure(EntityTypeBuilder<ProductAttributeValue> builder)
        {
            builder.ToTable("ProductAttributeValues");

            builder.HasKey(pav => pav.Id);

            builder.Property(pav => pav.Value).IsRequired().HasMaxLength(100);
            builder.Property(pav => pav.DisplayOrder).HasDefaultValue(0);

            builder.HasIndex(pav => new { pav.AttributeId, pav.Value })
                   .IsUnique()
                   .HasDatabaseName("UX_PAV_Attribute_Value");

            // Attribute relationship is configured in ProductAttributeConfiguration (HasMany side)

            // Many-to-many with ProductVariant is configured in ProductVariantConfiguration
        }
    }
}
