using E_LaptopShop.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace E_LaptopShop.Infra.Data.Configurations
{
    public sealed class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.ToTable("Users");

            builder.HasKey(u => u.Id)
                   .HasName("PK__Users__3214EC07F0A62389");

            // Required fields
            builder.Property(u => u.FirstName).IsRequired().HasMaxLength(50);
            builder.Property(u => u.LastName).IsRequired().HasMaxLength(50);
            builder.Property(u => u.Email).IsRequired().HasMaxLength(100);

            // Optional fields
            builder.Property(u => u.PasswordHash).HasMaxLength(255);
            builder.Property(u => u.Phone).HasMaxLength(20);
            builder.Property(u => u.AvatarUrl).HasMaxLength(255);
            builder.Property(u => u.Token).HasMaxLength(255);
            builder.Property(u => u.RefreshToken).HasMaxLength(512);
            builder.Property(u => u.VerificationToken).HasMaxLength(100);
            builder.Property(u => u.Gender).HasMaxLength(50);
            builder.Property(u => u.CreatedBy).HasMaxLength(100);
            builder.Property(u => u.UpdatedBy).HasMaxLength(100);

            // Defaults
            builder.Property(u => u.CreatedAt)
                   .HasColumnType("datetime")
                   .HasDefaultValueSql("(getdate())");

            builder.Property(u => u.IsActive).HasDefaultValue(true);
            builder.Property(u => u.EmailConfirmed).HasDefaultValue(false);
            builder.Property(u => u.LoginAttempts).HasDefaultValue(0);
            builder.Property(u => u.IsLocked).HasDefaultValue(false);

            // Ignore computed property (just in case EF tries to map it)
            builder.Ignore(u => u.FullName);

            // Relationship
            builder.HasOne(u => u.Role)
                   .WithMany(r => r.Users)
                   .HasForeignKey(u => u.RoleId)
                   .OnDelete(DeleteBehavior.ClientSetNull)
                   .HasConstraintName("FK_Users_Roles");

            // Indexes
            builder.HasIndex(u => u.Email)
                   .IsUnique()
                   .HasDatabaseName("UQ__Users__A9D10534BCF0AE75");

            builder.HasIndex(u => u.RoleId).HasDatabaseName("IX_Users_RoleId");
            builder.HasIndex(u => u.IsActive).HasDatabaseName("IX_Users_IsActive");
            builder.HasIndex(u => u.LastLoginAt).HasDatabaseName("IX_Users_LastLoginAt");
        }
    }
}
