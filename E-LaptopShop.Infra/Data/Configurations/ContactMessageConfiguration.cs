using E_LaptopShop.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace E_LaptopShop.Infra.Data.Configurations
{
    public sealed class ContactMessageConfiguration : IEntityTypeConfiguration<ContactMessage>
    {
        public void Configure(EntityTypeBuilder<ContactMessage> builder)
        {
            builder.ToTable("ContactMessages");

            builder.HasKey(cm => cm.Id);

            builder.Property(cm => cm.FullName).IsRequired().HasMaxLength(100);
            builder.Property(cm => cm.Email).IsRequired().HasMaxLength(100);
            builder.Property(cm => cm.Phone).HasMaxLength(20);
            builder.Property(cm => cm.Subject).HasMaxLength(200);
            builder.Property(cm => cm.Message).IsRequired().HasColumnType("nvarchar(max)");
            builder.Property(cm => cm.HandledBy).HasMaxLength(100);

            builder.Property(cm => cm.CreatedAt)
                   .HasDefaultValueSql("(sysutcdatetime())");
        }
    }
}
