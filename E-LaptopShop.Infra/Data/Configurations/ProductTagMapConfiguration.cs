using E_LaptopShop.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace E_LaptopShop.Infra.Data.Configurations
{
    public sealed class ProductTagMapConfiguration : IEntityTypeConfiguration<ProductTagMap>
    {
        public void Configure(EntityTypeBuilder<ProductTagMap> builder)
        {
            builder.ToTable("ProductTagMap");

            builder.HasKey(ptm => new { ptm.ProductId, ptm.TagId });

            // Tag relationship is configured in ProductTagConfiguration (HasMany side)

            builder.HasOne(ptm => ptm.Product)
                   .WithMany(p => p.ProductTagMaps)
                   .HasForeignKey(ptm => ptm.ProductId)
                   .HasConstraintName("FK_ProductTagMap_Products");
        }
    }
}
