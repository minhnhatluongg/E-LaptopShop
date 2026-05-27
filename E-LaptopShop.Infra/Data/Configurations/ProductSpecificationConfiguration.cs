using E_LaptopShop.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace E_LaptopShop.Infra.Data.Configurations
{
    public sealed class ProductSpecificationConfiguration : IEntityTypeConfiguration<ProductSpecification>
    {
        public void Configure(EntityTypeBuilder<ProductSpecification> builder)
        {
            builder.ToTable("ProductSpecifications");

            builder.HasKey(ps => ps.Id)
                   .HasName("PK__ProductS__3214EC077A3B327A");

            builder.Property(ps => ps.CPU).HasMaxLength(200);
            builder.Property(ps => ps.RAM).HasMaxLength(100);
            builder.Property(ps => ps.Storage).HasMaxLength(200);
            builder.Property(ps => ps.GPU).HasMaxLength(200);
            builder.Property(ps => ps.Screen).HasMaxLength(200);
            builder.Property(ps => ps.OS).HasMaxLength(100);
            builder.Property(ps => ps.Ports).HasMaxLength(500);
            builder.Property(ps => ps.Weight).HasMaxLength(50);
            builder.Property(ps => ps.Battery).HasMaxLength(100);

            builder.HasOne(ps => ps.Product)
                   .WithMany(p => p.ProductSpecifications)
                   .HasForeignKey(ps => ps.ProductId)
                   .HasConstraintName("FK__ProductSp__Produ__49C3F6B7");
        }
    }
}
