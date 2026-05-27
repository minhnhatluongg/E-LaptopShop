using E_LaptopShop.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace E_LaptopShop.Infra.Data.Configurations
{
    public sealed class UserAddressConfiguration : IEntityTypeConfiguration<UserAddress>
    {
        public void Configure(EntityTypeBuilder<UserAddress> builder)
        {
            builder.ToTable("UserAddresses");

            builder.HasKey(ua => ua.Id)
                   .HasName("PK__UserAddr__3214EC07499E7131");

            builder.Property(ua => ua.FullName).HasMaxLength(100);
            builder.Property(ua => ua.Phone).HasMaxLength(20);
            builder.Property(ua => ua.AddressLine).HasMaxLength(255);
            builder.Property(ua => ua.City).HasMaxLength(100);
            builder.Property(ua => ua.District).HasMaxLength(100);
            builder.Property(ua => ua.Ward).HasMaxLength(100);
            builder.Property(ua => ua.CountryCode).HasMaxLength(5).HasDefaultValue("VN");
            builder.Property(ua => ua.PostalCode).HasMaxLength(20);

            builder.Property(ua => ua.IsDefault).HasDefaultValue(false);
            builder.Property(ua => ua.IsDeleted).HasDefaultValue(false);

            builder.Property(ua => ua.CreatedAt)
                   .HasColumnType("datetime")
                   .HasDefaultValueSql("(getutcdate())");

            builder.HasIndex(ua => new { ua.UserId, ua.IsDefault })
                   .HasFilter("[IsDefault] = 1")
                   .IsUnique()
                   .HasDatabaseName("UX_UserAddresses_DefaultPerUser");

            builder.HasOne(ua => ua.User)
                   .WithMany(u => u.UserAddresses)
                   .HasForeignKey(ua => ua.UserId)
                   .HasConstraintName("FK__UserAddre__UserI__5BE2A6F2");

            builder.HasMany(ua => ua.Orders)
                   .WithOne(o => o.ShippingAddress)
                   .HasForeignKey(o => o.ShippingAddressId)
                   .OnDelete(DeleteBehavior.SetNull);
        }
    }
}
