using E_LaptopShop.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace E_LaptopShop.Infra.Data.Configurations
{
    public sealed class PaymentTransactionConfiguration : IEntityTypeConfiguration<PaymentTransaction>
    {
        public void Configure(EntityTypeBuilder<PaymentTransaction> builder)
        {
            builder.ToTable("PaymentTransactions");

            builder.HasKey(pt => pt.Id)
                   .HasName("PK__PaymentT__3214EC072905DF2C");

            builder.Property(pt => pt.PaymentMethod).HasMaxLength(50);
            builder.Property(pt => pt.Status).HasMaxLength(50);

            builder.Property(pt => pt.TransactionDate)
                   .HasColumnType("datetime")
                   .HasDefaultValueSql("(getdate())");

            // Order relationship is configured in OrderConfiguration (HasMany side)

            builder.HasMany(pt => pt.RefundTransactions)
                   .WithOne(rt => rt.PaymentTransaction)
                   .HasForeignKey(rt => rt.PaymentTransactionId)
                   .OnDelete(DeleteBehavior.SetNull);
        }
    }
}
