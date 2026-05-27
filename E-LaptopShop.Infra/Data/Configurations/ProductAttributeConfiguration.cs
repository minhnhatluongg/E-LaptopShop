using E_LaptopShop.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace E_LaptopShop.Infra.Data.Configurations
{
    public sealed class ProductAttributeConfiguration : IEntityTypeConfiguration<ProductAttribute>
    {
        public void Configure(EntityTypeBuilder<ProductAttribute> builder)
        {
            builder.ToTable("ProductAttributes");

            builder.HasKey(pa => pa.Id);

            builder.Property(pa => pa.Name).IsRequired().HasMaxLength(50);
            builder.Property(pa => pa.Slug).IsRequired().HasMaxLength(80).IsUnicode(false);
            builder.Property(pa => pa.IsActive).HasDefaultValue(true);

            builder.HasIndex(pa => pa.Slug)
                   .IsUnique()
                   .HasDatabaseName("UX_ProductAttributes_Slug");

            builder.HasMany(pa => pa.ProductAttributeValues)
                   .WithOne(pav => pav.Attribute)
                   .HasForeignKey(pav => pav.AttributeId)
                   .HasConstraintName("FK_PAV_Attributes");
        }
    }
}
