using E_LaptopShop.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace E_LaptopShop.Infra.Data.Configurations
{
    public sealed class WalletTransactionConfiguration : IEntityTypeConfiguration<WalletTransaction>
    {
        public void Configure(EntityTypeBuilder<WalletTransaction> builder)
        {
            builder.ToTable("WalletTransactions");

            builder.HasKey(t => t.Id)
                   .HasName("PK_WalletTransactions");

            builder.Property(t => t.Type)
                   .HasConversion<int>()
                   .IsRequired();

            builder.Property(t => t.Amount).HasColumnType("decimal(18,2)").IsRequired();
            builder.Property(t => t.BalanceBefore).HasColumnType("decimal(18,2)").IsRequired();
            builder.Property(t => t.BalanceAfter).HasColumnType("decimal(18,2)").IsRequired();

            builder.Property(t => t.ReferenceType).HasMaxLength(50);
            builder.Property(t => t.ReferenceId).HasMaxLength(50);
            builder.Property(t => t.Note).HasMaxLength(500);
            builder.Property(t => t.CreatedBy).HasMaxLength(50);

            builder.Property(t => t.CreatedAt)
                   .HasColumnType("datetime2")
                   .HasDefaultValueSql("SYSUTCDATETIME()");

            builder.HasIndex(t => t.WalletId).HasDatabaseName("IX_WalletTransactions_WalletId");
            builder.HasIndex(t => t.UserId).HasDatabaseName("IX_WalletTransactions_UserId");
            builder.HasIndex(t => new { t.ReferenceType, t.ReferenceId })
                   .HasDatabaseName("IX_WalletTransactions_Reference");
            builder.HasIndex(t => t.CreatedAt).HasDatabaseName("IX_WalletTransactions_CreatedAt");

            builder.HasOne(t => t.Wallet)
                   .WithMany(w => w.Transactions)
                   .HasForeignKey(t => t.WalletId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(t => t.User)
                   .WithMany()
                   .HasForeignKey(t => t.UserId)
                   .OnDelete(DeleteBehavior.Restrict); // user delete không cascade qua ledger
        }
    }
}
