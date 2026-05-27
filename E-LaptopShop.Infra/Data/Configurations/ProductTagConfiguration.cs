using E_LaptopShop.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace E_LaptopShop.Infra.Data.Configurations
{
    public sealed class ProductTagConfiguration : IEntityTypeConfiguration<ProductTag>
    {
        public void Configure(EntityTypeBuilder<ProductTag> builder)
        {
            builder.ToTable("ProductTags");

            builder.HasKey(pt => pt.Id);

            builder.Property(pt => pt.Name).IsRequired().HasMaxLength(80);
            builder.Property(pt => pt.Slug).IsRequired().HasMaxLength(120).IsUnicode(false);
            builder.Property(pt => pt.IsActive).HasDefaultValue(true);

            builder.HasIndex(pt => pt.Slug)
                   .IsUnique()
                   .HasDatabaseName("UX_ProductTags_Slug");

            builder.HasMany(pt => pt.ProductTagMap)
                   .WithOne(ptm => ptm.Tag)
                   .HasForeignKey(ptm => ptm.TagId)
                   .HasConstraintName("FK_ProductTagMap_Tags");
        }
    }
}
