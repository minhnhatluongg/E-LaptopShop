using E_LaptopShop.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace E_LaptopShop.Infra.Data.Configurations
{
    public sealed class RefundTransactionConfiguration : IEntityTypeConfiguration<RefundTransaction>
    {
        public void Configure(EntityTypeBuilder<RefundTransaction> builder)
        {
            builder.ToTable("RefundTransactions");

            builder.HasKey(rt => rt.Id);

            builder.Property(rt => rt.Amount).HasColumnType("decimal(18,2)");
            builder.Property(rt => rt.Status).IsRequired().HasMaxLength(50).HasDefaultValue("Pending");
            builder.Property(rt => rt.RefundMethod).HasMaxLength(50);
            builder.Property(rt => rt.Notes).HasMaxLength(500);

            builder.Property(rt => rt.CreatedAt)
                   .HasDefaultValueSql("(sysutcdatetime())");

            builder.HasIndex(rt => rt.OrderId).HasDatabaseName("IX_RefundTransactions_OrderId");

            builder.HasOne(rt => rt.Order)
                   .WithMany(o => o.RefundTransactions)
                   .HasForeignKey(rt => rt.OrderId)
                   .OnDelete(DeleteBehavior.Restrict);

            // ReturnRequest relationship is configured in ReturnRequestConfiguration (HasMany side)
            // PaymentTransaction relationship is configured in PaymentTransactionConfiguration (HasMany side)
        }
    }
}
