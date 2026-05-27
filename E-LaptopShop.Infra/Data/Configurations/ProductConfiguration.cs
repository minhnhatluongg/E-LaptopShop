using E_LaptopShop.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace E_LaptopShop.Infra.Data.Configurations
{
    public sealed class ProductConfiguration : IEntityTypeConfiguration<Product>
    {
        public void Configure(EntityTypeBuilder<Product> builder)
        {
            builder.ToTable("Products");

            builder.HasKey(p => p.Id)
                   .HasName("PK__Products__3214EC078E5484EF");

            builder.Property(p => p.Name)
                   .IsRequired()
                   .HasMaxLength(150);

            builder.Property(p => p.Slug)
                   .HasMaxLength(200)
                   .IsUnicode(false)
                   .IsRequired();

            builder.Property(p => p.Description)
                   .HasColumnType("nvarchar(max)");

            builder.Property(p => p.Price)
                   .HasColumnType("decimal(18,2)");

            builder.Property(p => p.Discount)
                   .HasColumnType("decimal(5,2)")
                   .HasDefaultValue(0m);

            builder.Property(p => p.InStock)
                   .HasDefaultValue(0);

            builder.Property(p => p.IsActive)
                   .IsRequired()
                   .HasDefaultValue(true);

            builder.Property(p => p.CreatedAt)
                   .HasColumnType("datetime")
                   .HasDefaultValueSql("GETUTCDATE()");

            // Indexes
            builder.HasIndex(p => p.Slug).IsUnique();
            builder.HasIndex(p => p.CategoryId);
            builder.HasIndex(p => p.BrandId);
            builder.HasIndex(p => p.IsActive);

            // Relationships
            builder.HasOne(p => p.Category)
                   .WithMany(c => c.Products)
                   .HasForeignKey(p => p.CategoryId)
                   .HasConstraintName("FK__Products__Catego__4316F928");

            builder.HasOne(p => p.Brand)
                   .WithMany(b => b.Products)
                   .HasForeignKey(p => p.BrandId);
        }
    }
}
