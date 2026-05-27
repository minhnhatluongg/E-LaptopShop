using E_LaptopShop.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace E_LaptopShop.Infra.Data.Configurations
{
    public sealed class ReturnRequestConfiguration : IEntityTypeConfiguration<ReturnRequest>
    {
        public void Configure(EntityTypeBuilder<ReturnRequest> builder)
        {
            builder.ToTable("ReturnRequests");

            builder.HasKey(rr => rr.Id);

            builder.Property(rr => rr.Reason).IsRequired().HasMaxLength(255);
            builder.Property(rr => rr.Status).IsRequired().HasMaxLength(50).HasDefaultValue("Pending");
            builder.Property(rr => rr.ResolvedBy).HasMaxLength(100);
            builder.Property(rr => rr.Notes).HasMaxLength(1000);

            builder.Property(rr => rr.RequestedAt)
                   .HasDefaultValueSql("(sysutcdatetime())");

            builder.HasIndex(rr => rr.OrderId).HasDatabaseName("IX_ReturnRequests_OrderId");
            builder.HasIndex(rr => rr.UserId).HasDatabaseName("IX_ReturnRequests_UserId");

            builder.HasOne(rr => rr.Order)
                   .WithMany(o => o.ReturnRequests)
                   .HasForeignKey(rr => rr.OrderId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(rr => rr.User)
                   .WithMany(u => u.ReturnRequests)
                   .HasForeignKey(rr => rr.UserId)
                   .OnDelete(DeleteBehavior.SetNull);

            // OrderItem relationship is configured in OrderItemConfiguration (HasMany side)

            builder.HasMany(rr => rr.RefundTransactions)
                   .WithOne(rt => rt.ReturnRequest)
                   .HasForeignKey(rt => rt.ReturnRequestId)
                   .OnDelete(DeleteBehavior.SetNull)
                   .HasConstraintName("FK_RefundTransactions_Return");
        }
    }
}
