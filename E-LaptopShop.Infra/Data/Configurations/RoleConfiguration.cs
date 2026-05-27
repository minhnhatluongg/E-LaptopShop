using E_LaptopShop.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace E_LaptopShop.Infra.Data.Configurations
{
    public sealed class RoleConfiguration : IEntityTypeConfiguration<Role>
    {
        public void Configure(EntityTypeBuilder<Role> builder)
        {
            builder.ToTable("Roles");

            builder.HasKey(r => r.Id)
                   .HasName("PK__Roles__3214EC07A09B5C70");

            builder.Property(r => r.Name)
                   .IsRequired()
                   .HasMaxLength(100);

            builder.Property(r => r.Code)
                   .HasMaxLength(50)
                   .HasDefaultValue("");

            builder.Property(r => r.IsActive)
                   .HasDefaultValue(true);

            builder.HasData(
                new Role { Id = 4,  Name = "SUPER ADMIN", Code = "SUPER_ADMIN", IsActive = true },
                new Role { Id = 5,  Name = "Sales",       Code = "SALES",       IsActive = true },
                new Role { Id = 6,  Name = "Customer",    Code = "CUSTOMER",    IsActive = true },
                new Role { Id = 7,  Name = "Admin",       Code = "ADMIN",       IsActive = true },
                new Role { Id = 8,  Name = "Manager",     Code = "MANAGER",     IsActive = true },
                new Role { Id = 9,  Name = "Warehouse",   Code = "WAREHOUSE",   IsActive = true },
                new Role { Id = 10, Name = "Support",     Code = "SUPPORT",     IsActive = true },
                new Role { Id = 11, Name = "Moderator",   Code = "MODERATOR",   IsActive = true },
                new Role { Id = 12, Name = "VIP",         Code = "VIP",         IsActive = true },
                new Role { Id = 13, Name = "Partner",     Code = "PARTNER",     IsActive = true }
            );
        }
    }
}
