using E_LaptopShop.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace E_LaptopShop.Infra.Data.Configurations
{
    public sealed class UserWalletConfiguration : IEntityTypeConfiguration<UserWallet>
    {
        public void Configure(EntityTypeBuilder<UserWallet> builder)
        {
            builder.ToTable("UserWallets");

            builder.HasKey(w => w.Id)
                   .HasName("PK_UserWallets");

            builder.HasIndex(w => w.UserId)
                   .IsUnique()
                   .HasDatabaseName("UX_UserWallets_UserId");

            builder.Property(w => w.Balance)
                   .HasColumnType("decimal(18,2)")
                   .HasDefaultValue(0m);

            builder.Property(w => w.LifetimeTopUp)
                   .HasColumnType("decimal(18,2)")
                   .HasDefaultValue(0m);

            builder.Property(w => w.LifetimeSpent)
                   .HasColumnType("decimal(18,2)")
                   .HasDefaultValue(0m);

            builder.Property(w => w.IsActive).HasDefaultValue(true);
            builder.Property(w => w.IsLocked).HasDefaultValue(false);
            builder.Property(w => w.LockReason).HasMaxLength(255);

            builder.Property(w => w.CreatedAt)
                   .HasColumnType("datetime2")
                   .HasDefaultValueSql("SYSUTCDATETIME()");

            builder.Property(w => w.UpdatedAt).HasColumnType("datetime2");

            // Concurrency token — chống race condition khi nhiều thread cùng update Balance
            builder.Property(w => w.RowVersion)
                   .IsRowVersion()
                   .IsConcurrencyToken();

            // 1-1 với User (UserId là FK, unique)
            builder.HasOne(w => w.User)
                   .WithOne()
                   .HasForeignKey<UserWallet>(w => w.UserId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
