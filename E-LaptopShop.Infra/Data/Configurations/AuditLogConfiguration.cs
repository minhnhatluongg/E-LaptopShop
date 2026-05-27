using E_LaptopShop.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace E_LaptopShop.Infra.Data.Configurations
{
    public sealed class AuditLogConfiguration : IEntityTypeConfiguration<AuditLog>
    {
        public void Configure(EntityTypeBuilder<AuditLog> builder)
        {
            builder.ToTable("AuditLogs");

            builder.HasKey(al => al.Id);

            builder.Property(al => al.EntityName).IsRequired().HasMaxLength(100);
            builder.Property(al => al.EntityId).HasMaxLength(50);
            builder.Property(al => al.Action).IsRequired().HasMaxLength(20);
            builder.Property(al => al.OldValues).HasColumnType("nvarchar(max)");
            builder.Property(al => al.NewValues).HasColumnType("nvarchar(max)");
            builder.Property(al => al.IpAddress).HasMaxLength(45);
            builder.Property(al => al.UserAgent).HasMaxLength(255);

            builder.Property(al => al.CreatedAt)
                   .HasDefaultValueSql("(sysutcdatetime())");

            builder.HasIndex(al => al.CreatedAt)
                   .HasDatabaseName("IX_AuditLogs_CreatedAt");

            builder.HasIndex(al => new { al.EntityName, al.EntityId })
                   .HasDatabaseName("IX_AuditLogs_Entity");

            builder.HasOne(al => al.User)
                   .WithMany(u => u.AuditLogs)
                   .HasForeignKey(al => al.UserId)
                   .OnDelete(DeleteBehavior.SetNull);
        }
    }
}
