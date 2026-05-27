using E_LaptopShop.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace E_LaptopShop.Infra.Data.Configurations
{
    public sealed class LoyaltyTierConfiguration : IEntityTypeConfiguration<LoyaltyTier>
    {
        public void Configure(EntityTypeBuilder<LoyaltyTier> builder)
        {
            builder.ToTable("LoyaltyTiers");

            builder.HasKey(lt => lt.Id);

            builder.Property(lt => lt.Name).IsRequired().HasMaxLength(50);
            builder.Property(lt => lt.MinSpend).HasColumnType("decimal(18,2)");
            builder.Property(lt => lt.DiscountPercent).HasColumnType("decimal(5,2)");
            builder.Property(lt => lt.PointsMultiplier).HasColumnType("decimal(5,2)").HasDefaultValue(1m);
            builder.Property(lt => lt.IsActive).HasDefaultValue(true);

            builder.HasIndex(lt => lt.Name)
                   .IsUnique()
                   .HasDatabaseName("UX_LoyaltyTiers_Name");

            builder.HasMany(lt => lt.UserLoyalty)
                   .WithOne(ul => ul.Tier)
                   .HasForeignKey(ul => ul.TierId)
                   .OnDelete(DeleteBehavior.ClientSetNull)
                   .HasConstraintName("FK_UserLoyalty_Tier");
        }
    }
}
