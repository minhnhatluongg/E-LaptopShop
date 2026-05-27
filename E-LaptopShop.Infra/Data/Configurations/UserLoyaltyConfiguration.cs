using E_LaptopShop.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace E_LaptopShop.Infra.Data.Configurations
{
    public sealed class UserLoyaltyConfiguration : IEntityTypeConfiguration<UserLoyalty>
    {
        public void Configure(EntityTypeBuilder<UserLoyalty> builder)
        {
            builder.ToTable("UserLoyalty");

            builder.HasKey(ul => ul.UserId);
            builder.Property(ul => ul.UserId).ValueGeneratedNever();

            builder.Property(ul => ul.LifetimeSpend).HasColumnType("decimal(18,2)");

            builder.Property(ul => ul.UpdatedAt)
                   .HasDefaultValueSql("(sysutcdatetime())");

            // Tier relationship is configured in LoyaltyTierConfiguration (HasMany side)

            builder.HasOne(ul => ul.User)
                   .WithOne(u => u.Loyalty)
                   .HasForeignKey<UserLoyalty>(ul => ul.UserId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
