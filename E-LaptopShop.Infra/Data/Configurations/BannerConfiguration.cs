using E_LaptopShop.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace E_LaptopShop.Infra.Data.Configurations
{
    public sealed class BannerConfiguration : IEntityTypeConfiguration<Banner>
    {
        public void Configure(EntityTypeBuilder<Banner> builder)
        {
            builder.ToTable("Banners");

            builder.HasKey(b => b.Id);

            builder.Property(b => b.Title).IsRequired().HasMaxLength(150);
            builder.Property(b => b.Subtitle).HasMaxLength(255);
            builder.Property(b => b.ImageUrl).IsRequired().HasMaxLength(500);
            builder.Property(b => b.LinkUrl).HasMaxLength(500);

            builder.Property(b => b.Position)
                   .IsRequired()
                   .HasMaxLength(50)
                   .HasDefaultValue("HOMEPAGE_TOP");

            builder.Property(b => b.IsActive).HasDefaultValue(true);

            builder.Property(b => b.CreatedAt)
                   .HasDefaultValueSql("(sysutcdatetime())");
        }
    }
}
