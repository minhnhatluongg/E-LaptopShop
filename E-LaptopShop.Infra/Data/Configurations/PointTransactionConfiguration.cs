using E_LaptopShop.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace E_LaptopShop.Infra.Data.Configurations
{
    public sealed class PointTransactionConfiguration : IEntityTypeConfiguration<PointTransaction>
    {
        public void Configure(EntityTypeBuilder<PointTransaction> builder)
        {
            builder.ToTable("PointTransactions");

            builder.HasKey(pt => pt.Id);

            builder.Property(pt => pt.Reason).IsRequired().HasMaxLength(100);

            builder.Property(pt => pt.CreatedAt)
                   .HasDefaultValueSql("(sysutcdatetime())");

            builder.HasIndex(pt => pt.UserId).HasDatabaseName("IX_PointTransactions_UserId");
            builder.HasIndex(pt => pt.OrderId).HasDatabaseName("IX_PointTransactions_OrderId");

            builder.HasOne(pt => pt.User)
                   .WithMany(u => u.PointTransactions)
                   .HasForeignKey(pt => pt.UserId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(pt => pt.Order)
                   .WithMany(o => o.PointTransactions)
                   .HasForeignKey(pt => pt.OrderId)
                   .OnDelete(DeleteBehavior.SetNull);
        }
    }
}
