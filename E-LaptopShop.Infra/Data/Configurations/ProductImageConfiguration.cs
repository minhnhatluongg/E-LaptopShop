using E_LaptopShop.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace E_LaptopShop.Infra.Data.Configurations
{
    public sealed class ProductImageConfiguration : IEntityTypeConfiguration<ProductImage>
    {
        public void Configure(EntityTypeBuilder<ProductImage> builder)
        {
            builder.ToTable("ProductImages");

            builder.HasKey(pi => pi.Id)
                   .HasName("PK__ProductI__3214EC078C5F1344");

            builder.Property(pi => pi.ImageUrl).IsRequired().HasMaxLength(255);
            builder.Property(pi => pi.FileType).IsRequired().HasMaxLength(50);
            builder.Property(pi => pi.FileSize).IsRequired().HasDefaultValue(0L);
            builder.Property(pi => pi.AltText).HasMaxLength(255);
            builder.Property(pi => pi.Title).HasMaxLength(100);
            builder.Property(pi => pi.CreatedBy).HasMaxLength(50);

            builder.Property(pi => pi.IsMain).HasDefaultValue(false);
            builder.Property(pi => pi.DisplayOrder).HasDefaultValue(0);
            builder.Property(pi => pi.IsActive).HasDefaultValue(true);

            builder.Property(pi => pi.UploadedAt)
                   .IsRequired()
                   .HasColumnType("datetime")
                   .HasDefaultValueSql("GETDATE()");

            builder.Property(pi => pi.CreatedAt)
                   .IsRequired()
                   .HasColumnType("datetime")
                   .HasDefaultValueSql("GETDATE()");

            builder.HasOne(pi => pi.Product)
                   .WithMany(p => p.ProductImages)
                   .HasForeignKey(pi => pi.ProductId)
                   .OnDelete(DeleteBehavior.Cascade)
                   .HasConstraintName("FK_ProductImages_Products");

            builder.HasOne(pi => pi.SysFile)
                   .WithMany(sf => sf.ProductImages)
                   .HasForeignKey(pi => pi.SysFileId)
                   .OnDelete(DeleteBehavior.SetNull)
                   .HasConstraintName("FK_ProductImages_SysFiles");

            builder.HasIndex(pi => pi.ProductId)
                   .HasDatabaseName("IX_ProductImages_ProductId");

            builder.HasIndex(pi => new { pi.ProductId, pi.IsMain })
                   .HasDatabaseName("IX_ProductImages_ProductId_IsMain");
        }
    }
}
