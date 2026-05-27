using E_LaptopShop.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace E_LaptopShop.Infra.Data.Configurations
{
    public sealed class SysFileConfiguration : IEntityTypeConfiguration<SysFile>
    {
        public void Configure(EntityTypeBuilder<SysFile> builder)
        {
            builder.ToTable("SysFile");

            builder.HasKey(sf => sf.Id)
                   .HasName("PK__SysFiles__3214EC07");

            builder.Property(sf => sf.FileName).IsRequired().HasMaxLength(255);
            builder.Property(sf => sf.FilePath).IsRequired().HasMaxLength(255);
            builder.Property(sf => sf.FileUrl).IsRequired().HasMaxLength(255);
            builder.Property(sf => sf.FileType).IsRequired().HasMaxLength(50);
            builder.Property(sf => sf.StorageType).IsRequired().HasMaxLength(100).HasDefaultValue("local");
            builder.Property(sf => sf.UploadedBy).HasMaxLength(50);

            builder.Property(sf => sf.IsActive).HasDefaultValue(true);

            builder.Property(sf => sf.UploadedAt)
                   .HasColumnType("datetime")
                   .HasDefaultValueSql("(getdate())");
        }
    }
}
