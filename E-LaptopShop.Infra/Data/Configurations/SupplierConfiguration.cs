using E_LaptopShop.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace E_LaptopShop.Infra.Data.Configurations
{
    public sealed class SupplierConfiguration : IEntityTypeConfiguration<Supplier>
    {
        public void Configure(EntityTypeBuilder<Supplier> builder)
        {
            builder.ToTable("Suppliers");

            builder.HasKey(s => s.Id)
                   .HasName("PK__Suppliers__3214EC07");

            builder.Property(s => s.Name).IsRequired().HasMaxLength(200);
            builder.Property(s => s.ContactName).HasMaxLength(100);
            builder.Property(s => s.Email).HasMaxLength(100);
            builder.Property(s => s.Phone).HasMaxLength(20);
            builder.Property(s => s.Address).HasMaxLength(500);
            builder.Property(s => s.Website).HasMaxLength(255);
            builder.Property(s => s.Notes).HasMaxLength(1000);

            builder.Property(s => s.IsActive).HasDefaultValue(true);

            builder.Property(s => s.CreatedAt)
                   .HasColumnType("datetime")
                   .HasDefaultValueSql("(getdate())");

            builder.Property(s => s.UpdatedAt).HasColumnType("datetime");

            builder.HasMany(s => s.Orders)
                   .WithOne(so => so.Supplier)
                   .HasForeignKey(so => so.SupplierId)
                   .HasConstraintName("FK_SupplierOrders_Suppliers");
        }
    }
}
