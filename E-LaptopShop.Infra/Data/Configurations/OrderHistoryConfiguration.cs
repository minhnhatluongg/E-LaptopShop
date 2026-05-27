using E_LaptopShop.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace E_LaptopShop.Infra.Data.Configurations
{
    public sealed class OrderHistoryConfiguration : IEntityTypeConfiguration<OrderHistory>
    {
        public void Configure(EntityTypeBuilder<OrderHistory> builder)
        {
            builder.ToTable("OrderHistories");

            builder.HasKey(oh => oh.Id)
                   .HasName("PK__OrderHistories__3214EC07");

            builder.Property(oh => oh.OldStatus).IsRequired().HasMaxLength(50);
            builder.Property(oh => oh.NewStatus).IsRequired().HasMaxLength(50);

            builder.Property(oh => oh.ChangedAt)
                   .HasColumnType("datetime")
                   .HasDefaultValueSql("(getdate())");

            builder.Property(oh => oh.ChangedBy).HasMaxLength(100);
            builder.Property(oh => oh.Notes).HasMaxLength(500);

            // Order relationship is configured in OrderConfiguration (HasMany side)
        }
    }
}
