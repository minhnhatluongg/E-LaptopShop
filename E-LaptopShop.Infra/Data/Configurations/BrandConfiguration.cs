using E_LaptopShop.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace E_LaptopShop.Infra.Data.Configurations
{
    /// <summary>
    /// Fluent API configuration for <see cref="Brand"/>.
    /// Auto-discovered via <c>modelBuilder.ApplyConfigurationsFromAssembly(...)</c>.
    /// </summary>
    public sealed class BrandConfiguration : IEntityTypeConfiguration<Brand>
    {
        public void Configure(EntityTypeBuilder<Brand> builder)
        {
            builder.ToTable("Brands");

            builder.HasKey(b => b.Id)
                   .HasName("PK__Brands__3214EC07");

            builder.Property(b => b.Name)
                   .IsRequired()
                   .HasMaxLength(100);

            builder.Property(b => b.Description)
                   .HasColumnType("nvarchar(max)");

            builder.Property(b => b.Slug)
                   .HasMaxLength(200)
                   .IsUnicode(false);

            builder.Property(b => b.IsActive)
                   .IsRequired()
                   .HasDefaultValue(true);

            builder.Property(b => b.CreatedAt)
                   .HasColumnType("datetime")
                   .HasDefaultValueSql("GETUTCDATE()");

            // Indexes
            builder.HasIndex(b => b.Slug)
                   .IsUnique()
                   .HasDatabaseName("UX_Brands_Slug")
                   .HasFilter("[Slug] IS NOT NULL");

            // Relationships are configured on the dependent side (Product → Brand).
        }
    }
}
